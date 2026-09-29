using System;
using System.IO.Ports;
using System.Threading;
using Monopoly.Comunicacion;
using Monopoly.Juego;
using Monopoly.Protocolo;

namespace Monopoly
{
    public class LocalSerialService
    {
        private SerialPort? _puertoDado;
        private SerialPort? _puertoRfid;
        private readonly LectorRfidService _lectorRfidService;
        private readonly IProcesadorAcciones _procesador;
        private DateTime _ultimoTiempoLectura = DateTime.MinValue;

        public LocalSerialService(IProcesadorAcciones procesador, LectorRfidService lectorRfidService)
        {
            _procesador = procesador;
            _lectorRfidService = lectorRfidService;
        }

        public void Iniciar(string puertoDado, string puertoRfid)
        {
            // --- PUERTO DADO (Raspberry Pi Pico) ---
            try
            {
                _puertoDado = new SerialPort(puertoDado, 115200)
                {
                    DtrEnable = true,
                    RtsEnable = true,
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };

                if (_puertoDado.IsOpen)
                {
                    _puertoDado.Close();
                }

                _puertoDado.DataReceived += PuertoDado_DataReceived;
                _puertoDado.Open();

                _puertoDado.DiscardInBuffer();
                _puertoDado.DiscardOutBuffer();

                Console.WriteLine($"[USB Dado] Escuchando en: {puertoDado}");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"[ERROR USB Dado] Permiso denegado en {puertoDado}. Reconecte el cable USB de la Pico.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR USB Dado] No se pudo abrir {puertoDado}: {ex.Message}");
            }

            // --- PUERTO RFID ---
            try
            {
                _puertoRfid = new SerialPort(puertoRfid, 115200)
                {
                    DtrEnable = true,
                    RtsEnable = true,
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };

                _puertoRfid.DataReceived += PuertoRfid_DataReceived;
                _puertoRfid.Open();
                _puertoRfid.DiscardInBuffer();
                _puertoRfid.DiscardOutBuffer();

                Console.WriteLine($"[USB RFID] Escuchando en: {puertoRfid} (115200 Baud)");

                InicializarYBuscarTarjetasPN532();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR USB RFID] No se pudo abrir {puertoRfid}: {ex.Message}");
            }
        }

        private void InicializarYBuscarTarjetasPN532()
        {
            if (_puertoRfid == null || !_puertoRfid.IsOpen) return;

            try
            {
                byte[] wakeup = new byte[] { 0x55, 0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x03, 0xFD, 0xD4, 0x14, 0x01, 0x17, 0x00 };
                _puertoRfid.Write(wakeup, 0, wakeup.Length);
                Thread.Sleep(50);

                byte[] samConfig = new byte[] { 0x00, 0x00, 0xFF, 0x03, 0xFD, 0xD4, 0x14, 0x01, 0x17, 0x00 };
                _puertoRfid.Write(samConfig, 0, samConfig.Length);
                Thread.Sleep(50);

                SolicitarLecturaTarjetaPN532();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR Init PN532] {ex.Message}");
            }
        }

        public void SolicitarLecturaTarjetaPN532()
        {
            if (_puertoRfid != null && _puertoRfid.IsOpen)
            {
                byte[] pollCard = new byte[] { 0x00, 0x00, 0xFF, 0x04, 0xFC, 0xD4, 0x4A, 0x01, 0x00, 0xE1, 0x00 };
                _puertoRfid.Write(pollCard, 0, pollCard.Length);
            }
        }

        private void PuertoDado_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_puertoDado == null || !_puertoDado.IsOpen) return;

            try
            {
                string linea = _puertoDado.ReadLine().Trim();
                if (linea.StartsWith("DADO:"))
                {
                    string valorDadoStr = linea.Replace("DADO:", "").Trim();
                    if (int.TryParse(valorDadoStr, out int valorDado))
                    {
                        Console.WriteLine($"\n[USB Dado] Número del Dado: {valorDado}");

                        // Obtener al jugador en turno dinámicamente si el procesador es de tipo ProcesadorJuego
                        int idJugadorActual = 1;
                        string nombreJugadorActual = "Jugador";

                        if (_procesador is ProcesadorJuego procesadorConcreto)
                        {
                            var jugador = procesadorConcreto.ObtenerJugadorActual();
                            if (jugador != null)
                            {
                                idJugadorActual = jugador.Id;
                                nombreJugadorActual = jugador.Nombre;
                            }
                        }

                        var mensaje = new Mensaje
                        {
                            Accion = TipoAccion.TIRAR_DADOS,
                            IdJugador = idJugadorActual,
                            Descripcion = valorDado.ToString()
                        };

                        // Procesar la tirada
                        var respuesta = _procesador.Procesar(mensaje);

                        if (respuesta != null && respuesta.Exito == true)
                        {
                        Console.Write("\n ¿Deseas comprar la propiedad? (S/N) y presiona Enter: ");
                        string respuestaDecision = Console.ReadLine()?.Trim().ToUpper()?? "N";

                        if (respuestaDecision == "S")
                            {
                                Console.WriteLine("\n --> OPCION 'S' SELECCIONADA <-- ");
                                Console.WriteLine(" --> ACERCA TU LLAVERO RFID AL LECTOR PARA FINALIZAR LA COMPRA <-- \n");
                            }
                            else
                            {
                                Console.WriteLine($"\n[INFO] {nombreJugadorActual} decidió no comprar. ");

                                //Cambio de turno
                                var mensajeFin = new Mensaje
                                {
                                    Accion = TipoAccion.TERMINAR_TURNO,
                                    IdJugador = idJugadorActual
                                };

                                var respFin = _procesador.Procesar(mensajeFin);
                                Console.WriteLine($"[TURNO] {respFin?.Descripcion}\n");
                            }
                        }
                    }
                }
            }
            catch (TimeoutException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error Lectura Dado]: {ex.Message}");
            }
        }

        private void PuertoRfid_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_puertoRfid == null || !_puertoRfid.IsOpen) return;

            try
            {
                int bytesDisponibles = _puertoRfid.BytesToRead;
                if (bytesDisponibles < 15) return;

                byte[] buffer = new byte[bytesDisponibles];
                _puertoRfid.Read(buffer, 0, bytesDisponibles);

                string tramaHex = BitConverter.ToString(buffer).Replace("-", "");

                int index = tramaHex.IndexOf("D54B0101");
                if (index != -1 && tramaHex.Length >= index + 26)
                {
                    string uidLimpio = tramaHex.Substring(index + 18, 8);

                    if ((DateTime.Now - _ultimoTiempoLectura).TotalSeconds > 2)
                    {
                        _ultimoTiempoLectura = DateTime.Now;

                        Console.WriteLine($"\n[RFID EXITOSO] Tarjeta detectada con UID: {uidLimpio}");

                        _lectorRfidService.ProcesarLecturaReal(uidLimpio);
                    }
                }

                SolicitarLecturaTarjetaPN532();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR Lectura PN532] {ex.Message}");
            }
        }

        public void Detener()
        {
            if (_puertoDado != null && _puertoDado.IsOpen) _puertoDado.Close();
            if (_puertoRfid != null && _puertoRfid.IsOpen) _puertoRfid.Close();
            Console.WriteLine("[USB] Conexiones seriales cerradas.");
        }
    }
}