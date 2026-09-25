<<<<<<< Updated upstream
using Monopoly.Comunicacion;
using Monopoly.Protocolo;

// -------------------------------------------------------------------
// PRUEBA: esto se tiene que reemplazar por la lógica real 
// (validar turno, mover en el tablero, cobrar alquiler,
// etc. usando Banco, Jugador, Tablero...). De momento, es para
// probar que la comunicación cliente-servidor funciona de verdad.
// -------------------------------------------------------------------
class ProcesadorDePrueba : IProcesadorAcciones
{
    public Mensaje Procesar(Mensaje solicitud)
    {
        Console.WriteLine($"[Servidor] Jugador {solicitud.IdJugador} pidió {solicitud.Accion}");

        // TODO: aquí es donde se conecta Banco/Tablero/Jugador
        // para validar y aplicar de verdad la acción.
        return new Mensaje
        {
            Accion = solicitud.Accion,
            IdJugador = solicitud.IdJugador,
            Exito = true,
            Descripcion = $"Acción {solicitud.Accion} recibida (stub, aún sin lógica real)"
        };
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Uso:
        //   dotnet run servidor
        //   dotnet run cliente <ip> <idJugador>
        if (args.Length == 0)
        {
            Console.WriteLine("Uso: dotnet run [servidor | cliente <ip> <idJugador>]");
            return;
        }

        if (args[0] == "servidor")
        {
            var servidor = new Servidor(5000, new ProcesadorDePrueba());
            servidor.Iniciar();
        }
        else if (args[0] == "cliente")
        {
            string ip = args.Length > 1 ? args[1] : "127.0.0.1";
            int idJugador = args.Length > 2 ? int.Parse(args[2]) : 1;

            var cliente = new Cliente(ip, 5000, idJugador);

            // Cuando llegue cualquier mensaje del servidor, lo imprimimos.
            // Godot haría algo similar, pero actualizando la pantalla en
            // vez de imprimir en consola.
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
    }
}
=======
﻿using Monopoly.Juego;
using Monopoly.Modelos;

Juego juego = new Juego();


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
    Console.WriteLine(
        "Posición: " +
        jugador.Posicion?.Dato.Nombre
    );
  

    Console.WriteLine();
    Console.WriteLine("Presiona ENTER para lanzar los dados...");
    Console.ReadLine();

    juego.TirarDados();

    Console.WriteLine();
    Console.WriteLine(
        "Posición actual: " +
        jugador.Posicion?.Dato.Nombre
    );

    Console.WriteLine(
        "Saldo actual: " +
        jugador.Saldo
    );

    // Preguntar si cayó en propiedad disponible
    if (
        jugador.Posicion != null &&
        jugador.Posicion.Dato is Propiedad propiedad &&
        propiedad.EstaDisponible()
    )
    {
        Console.WriteLine();
        Console.WriteLine(
            "¿Deseas comprar " +
            propiedad.Nombre +
            " por " +
            propiedad.PrecioCompra +
            "?"
        );

        Console.WriteLine("1. Sí");
        Console.WriteLine("2. No");

        string? opcion = Console.ReadLine();

        if (opcion == "1")
        {
            juego.ComprarPropiedad();
        }
        else
        {
            Console.WriteLine(
                "No se compró la propiedad."
            );
        }
    }

    Console.WriteLine();
    Console.WriteLine(
        "Presiona ENTER para terminar el turno..."
    );

    Console.ReadLine();

    juego.TerminarTurno();
}

Console.WriteLine();
Console.WriteLine("        PARTIDA TERMINADA");

juego.MostrarResultadoFinal();

Console.WriteLine();
Console.WriteLine("Exportando transacciones...");

juego.Banco.ExportarHistorial(
    "transacciones.txt"
);
>>>>>>> Stashed changes
