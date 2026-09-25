using System;
using System.IO.Ports;
using System.Threading;
using Monopoly.Comunicacion;
using Monopoly.Protocolo;

namespace Monopoly
{
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
                Descripcion = $"Acción {solicitud.Accion} recibida"
            };
        }
    }

    public class LectorRfidService
    {
        private const string PuertoCom = "/dev/tty.usbserial-14610";
        private const int Baudios = 115200;

        public static void IniciarLectura()
        {
            byte[] wakeup = new byte[] {
                0x55, 0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0xFF, 0x03, 0xFD, 0xD4, 0x14, 0x01, 0x17, 0x00
            };

            byte[] buscarTarjeta = new byte[] {
                0x00, 0x00, 0xFF, 0x04, 0xFC, 0xD4, 0x4A, 0x01, 0x00, 0xE1, 0x00
            };

            try
            {
                using SerialPort puerto = new SerialPort(PuertoCom, Baudios, Parity.None, 8, StopBits.One);
                puerto.Open();
                Console.WriteLine($"Conectado al PN532 en {PuertoCom}");
                Console.WriteLine("Escaneando tarjetas NFC...\n");

                puerto.Write(wakeup, 0, wakeup.Length);
                Thread.Sleep(100);

                if (puerto.BytesToRead > 0)
                {
                    byte[] descarta = new byte[puerto.BytesToRead];
                    puerto.Read(descarta, 0, descarta.Length);
                }

                string ultimoUid = "";

                while (true)
                {
                    puerto.Write(buscarTarjeta, 0, buscarTarjeta.Length);
                    Thread.Sleep(150);

                    int bytes = puerto.BytesToRead;
                    if (bytes > 0)
                    {
                        byte[] respuesta = new byte[bytes];
                        puerto.Read(respuesta, 0, bytes);

                        if (respuesta.Length >= 20)
                        {
                            string uid = $"{respuesta[19]:X2}:{respuesta[20]:X2}:{respuesta[21]:X2}:{respuesta[22]:X2}";

                            if (uid != ultimoUid)
                            {
                                Console.WriteLine($"🎯 Tarjeta / Llavero Detectado!");
                                Console.WriteLine($"   UID: {uid}\n");
                                ultimoUid = uid;
                            }
                        }
                    }
                    else
                    {
                        ultimoUid = "";
                    }

                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en puerto serial: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Uso:");
                Console.WriteLine("  dotnet run servidor           (Para servidor TCP)");
                Console.WriteLine("  dotnet run cliente <ip> <id>  (Para cliente TCP)");
                Console.WriteLine("  dotnet run rfid               (Para probar lector RFID)");
                return;
            }

            string modo = args[0].ToLower();

            if (modo == "servidor")
            {
                var servidor = new Servidor(5000, new ProcesadorDePrueba());
                servidor.Iniciar();
            }
            else if (modo == "cliente")
            {
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
            }
            else if (modo == "rfid")
            {
                LectorRfidService.IniciarLectura();
            }
        }
    }
}