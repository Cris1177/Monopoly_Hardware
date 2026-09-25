using System;
using System.IO.Ports;
using Monopoly.Comunicacion;
using Monopoly.Protocolo;
using Monopoly.Juego;
using Monopoly.Modelos;

namespace Monopoly
{
    // ===================================================================
    // 1. PROCESADOR DE PRUEBA 
    // ===================================================================
    class ProcesadorDePrueba : IProcesadorAcciones
    {
        public Mensaje Procesar(Mensaje solicitud)
        {
            Console.WriteLine($"[Servidor] Jugador {solicitud.IdJugador} pidió {solicitud.Accion}");

            return new Mensaje
            {
                Accion = solicitud.Accion,
                IdJugador = solicitud.IdJugador,
                Exito = true,
                Descripcion = $"Acción {solicitud.Accion} recibida (stub, aún sin lógica real)"
            };
        }
    }

    // ===================================================================
    // 2. SERVICIO / LECTOR RFID 
    // ===================================================================
    public class LectorRfidService
    {
        private SerialPort? _serialPort;

        public void IniciarLector(string puertoNombre = "/dev/tty.usbserial-10", int baudRate = 115200)
        {
            try
            {
                _serialPort = new SerialPort(puertoNombre, baudRate, Parity.None, 8, StopBits.One);
                _serialPort.Open();
                Console.WriteLine($"[RFID] Puerto {puertoNombre} abierto correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RFID Error] No se pudo abrir el puerto: {ex.Message}");
            }
        }

        public void DetenerLector()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                Console.WriteLine("[RFID] Puerto cerrado.");
            }
        }
    }

    // ===================================================================
    // 3. PROGRAM PRINCIPAL
    // ===================================================================
    class Program
    {
        static void Main(string[] args)
        {
            // Opciones de ejecución:
            //   dotnet run local
            //   dotnet run servidor
            //   dotnet run cliente <ip> <idJugador>
            //   dotnet run rfid

            if (args.Length == 0)
            {
                Console.WriteLine("Uso:");
                Console.WriteLine("  dotnet run local                 -> Ejecuta la partida local de Monopoly");
                Console.WriteLine("  dotnet run servidor              -> Inicia el servidor de red");
                Console.WriteLine("  dotnet run cliente <ip> <id>     -> Conecta un cliente a la red");
                Console.WriteLine("  dotnet run rfid                  -> Prueba el lector RFID PN532");
                return;
            }

            string modo = args[0].ToLower();

            switch (modo)
            {
                case "local":
                    EjecutarJuegoLocal();
                    break;

                case "servidor":
                    var servidor = new Servidor(5000, new ProcesadorDePrueba());
                    servidor.Iniciar();
                    break;

                case "cliente":
                    string ip = args.Length > 1 ? args[1] : "127.0.0.1";
                    int idJugador = args.Length > 2 ? int.Parse(args[2]) : 1;

                    var cliente = new Cliente(ip, 5000, idJugador);
                    cliente.MensajeRecibido += mensaje =>
                    {
                        Console.WriteLine($"[Cliente {idJugador}] Recibido: {mensaje.Accion} - Exito: {mensaje.Exito} - {mensaje.Descripcion}");
                    };

                    cliente.Conectar();
                    Console.WriteLine("Conectado. Escribe una acción (ej: TIRAR_DADOS) o 'salir':");
                    string? entrada;
                    while ((entrada = Console.ReadLine()) != null && entrada != "salir")
                    {
                        if (Enum.TryParse<TipoAccion>(entrada, out var accion))
                        {
                            cliente.EnviarAccion(accion);
                        }
                        else
                        {
                            Console.WriteLine("Acción no reconocida.");
                        }
                    }
                    cliente.Desconectar();
                    break;

                case "rfid":
                    Console.WriteLine("Iniciando prueba de Lector RFID...");
                    var rfid = new LectorRfidService();
                    rfid.IniciarLector();
                    Console.WriteLine("Presiona ENTER para detener...");
                    Console.ReadLine();
                    rfid.DetenerLector();
                    break;

                default:
                    Console.WriteLine("Modo no reconocido.");
                    break;
            }
        }

        // -------------------------------------------------------------------
        // Lógica de Juego Local (Cris)
        // -------------------------------------------------------------------
        private static void EjecutarJuegoLocal()
        {
            Juego.Juego juego = new Juego.Juego();

            Console.WriteLine("PRUEBA LOCAL MONOPOLY");

            while (!juego.JuegoTerminado())
            {
                Jugador? jugador = juego.ObtenerJugadorActual();

                if (jugador == null)
                {
                    Console.WriteLine("No hay jugador actual.");
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("Turno: " + juego.NumeroTurno);
                Console.WriteLine("Jugador: " + jugador.Nombre);
                Console.WriteLine("Saldo: " + jugador.Saldo);
                Console.WriteLine("Posición: " + jugador.Posicion?.Dato.Nombre);

                Console.WriteLine();
                Console.WriteLine("Presiona ENTER para lanzar los dados...");
                Console.ReadLine();

                juego.TirarDados();

                Console.WriteLine();
                Console.WriteLine("Posición actual: " + jugador.Posicion?.Dato.Nombre);
                Console.WriteLine("Saldo actual: " + jugador.Saldo);

                // Preguntar si cayó en propiedad disponible
                if (jugador.Posicion != null &&
                    jugador.Posicion.Dato is Propiedad propiedad &&
                    propiedad.EstaDisponible())
                {
                    Console.WriteLine();
                    Console.WriteLine($"¿Deseas comprar {propiedad.Nombre} por {propiedad.PrecioCompra}?");
                    Console.WriteLine("1. Sí");
                    Console.WriteLine("2. No");

                    string? opcion = Console.ReadLine();

                    if (opcion == "1")
                    {
                        juego.ComprarPropiedad();
                    }
                    else
                    {
                        Console.WriteLine("No se compró la propiedad.");
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Presiona ENTER para terminar el turno...");
                Console.ReadLine();

                juego.TerminarTurno();
            }

            Console.WriteLine();
            Console.WriteLine("        PARTIDA TERMINADA");

            juego.MostrarResultadoFinal();

            Console.WriteLine();
            Console.WriteLine("Exportando transacciones...");
            juego.Banco.ExportarHistorial("transacciones.txt");
        }
    }
}