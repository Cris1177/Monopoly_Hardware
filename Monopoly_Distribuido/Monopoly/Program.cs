using System;
using Monopoly.Comunicacion;
using Monopoly.Juego;
using Monopoly.Modelos;
using Monopoly.Protocolo;

namespace Monopoly
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Uso:");
                Console.WriteLine("  dotnet run --project Monopoly.csproj local                 -> Partida local");
                Console.WriteLine("  dotnet run --project Monopoly.csproj hardware              -> Partida Hardware USB (Pico + USB)");
                Console.WriteLine("  dotnet run --project Monopoly.csproj servidor              -> Servidor TCP");
                Console.WriteLine("  dotnet run --project Monopoly.csproj cliente <ip> <id>     -> Cliente TCP");
                Console.WriteLine("  dotnet run --project Monopoly.csproj rfid                  -> Lector RFID");
                return;
            }

            string modo = args[0].ToLower();

            switch (modo)
            {
                case "local":
                    EjecutarJuegoLocal();
                    break;

                case "servidor":
                    var juego = new Juego.Juego();
                    IProcesadorAcciones procesador = new ProcesadorJuego(juego);
                    var servidor = new Servidor(5000, procesador);
                    servidor.Iniciar();
                    break;

                case "cliente":
                    string ip = args.Length > 1 ? args[1] : "127.0.0.1";
                    int idJugador = args.Length > 2 ? int.Parse(args[2]) : 1;

                    var cliente = new Cliente(ip, 5000, idJugador);
                    cliente.MensajeRecibido += mensaje =>
                    {
                        Console.WriteLine($"[Cliente {idJugador}] Recibido: {mensaje.Accion} - Éxito: {mensaje.Exito} - {mensaje.Descripcion}");
                    };

                    cliente.Conectar();
                    Console.WriteLine($"Conectado como Jugador {idJugador}. Escribe una acción o 'salir':");

                    string? entrada;
                    while ((entrada = Console.ReadLine()) != null && entrada.ToLower() != "salir")
                    {
                        if (Enum.TryParse<TipoAccion>(entrada, true, out var accion))
                        {
                            cliente.EnviarAccion(accion);
                        }
                        else
                        {
                            Console.WriteLine("Acción no válida. Opciones: TIRAR_DADOS, COMPRAR_PROPIEDAD, TERMINAR_TURNO, PAGAR");
                        }
                    }
                    cliente.Desconectar();
                    break;


                case "hardware":
                    Console.WriteLine("Iniciando Monopoly en Modo Hardware USB...");
    
                    // 1. Instanciar el juego y su procesador de acciones
                    var juegoHardware = new Juego.Juego();
                    IProcesadorAcciones procesadorHardware = new ProcesadorJuego(juegoHardware);

                    // 2. Instanciar el cliente TCp o bridge para el lector RFID
                    var lectorRfidHardware = new LectorRfidService(procesadorHardware);

                    // 3. Iniciar el USB Serial
                    var servicioUsb = new LocalSerialService(procesadorHardware, lectorRfidHardware);


                    string puertoRaspberry = "/dev/tty.usbmodem144101";
                    string puertoRfid = "/dev/cu.usbserial-1410";

                    servicioUsb.Iniciar(puertoRaspberry, puertoRfid);

                    Console.WriteLine("\nSISTEMA LISTO. Esperando entrada del dado (Raspberry) y cobros (RFID)...");
                    Console.WriteLine("Escribe 'EXIT' y presiona ENTER para detener el sistema.\n");

                    // En lugar de un ReadLine() simple que se activa con cualquier '1' o número,
                    // esperamos explícitamente a que el usuario escriba EXIT.
                    while (true)
                    {
                        string? input = Console.ReadLine();
                        if (input != null && input.Trim().ToUpper() == "EXIT")
                        {
                            break;
                        }
                    }

    servicioUsb.Detener();
    break;


                case "rfid":
                    Console.WriteLine("Iniciando servicio de Lector RFID en red...");
                    var clienteRfid = new Cliente("127.0.0.1", 5000, 99);
                    clienteRfid.Conectar();

                    var lectorRfid = new LectorRfidService(clienteRfid);
                    lectorRfid.Iniciar();

                    Console.WriteLine("Lector RFID activo en segundo plano enviando pagos. Presiona ENTER para salir...");
                    Console.ReadLine();

                    lectorRfid.Detener();
                    clienteRfid.Desconectar();
                    break;


                default:
                    Console.WriteLine("Modo no reconocido.");
                    break;
            }
        }

        private static void EjecutarJuegoLocal()
        {
            Juego.Juego juego = new Juego.Juego();
            Console.WriteLine("PRUEBA LOCAL MONOPOLY");

            while (!juego.JuegoTerminado())
            {
                Jugador? jugador = juego.ObtenerJugadorActual();
                if (jugador == null) break;

                Console.WriteLine($"\nTurno: {juego.NumeroTurno} | Jugador: {jugador.Nombre} | Saldo: {jugador.Saldo}");
                Console.WriteLine("Presiona ENTER para lanzar los dados...");
                Console.ReadLine();

                juego.TirarDados();

                if (jugador.Posicion?.Dato is Propiedad propiedad && propiedad.EstaDisponible())
                {
                    Console.WriteLine($"¿Deseas comprar {propiedad.Nombre} por {propiedad.PrecioCompra}? (1: Sí / 2: No)");
                    if (Console.ReadLine() == "1")
                    {
                        juego.ComprarPropiedad();
                    }
                }

                Console.WriteLine("Presiona ENTER para terminar el turno...");
                Console.ReadLine();
                juego.TerminarTurno();
            }

            Console.WriteLine("\nPARTIDA TERMINADA");
            juego.MostrarResultadoFinal();
            juego.Banco.ExportarHistorial("transacciones.txt");
        }
    }
}
