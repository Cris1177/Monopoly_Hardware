namespace Monopoly.Juego;

public class Juego
{
    public Tablero Tablero { get; }
    public Banco Banco { get; }
    public Dado Dado { get; }
    public List<Jugador> Jugadores { get; }
    public ColaCircular<Jugador> Turnos { get; }
    public int Turno { get; private set; }

    public Juego()
    {
        Tablero = new Tablero();
        Banco = new Banco();
        Dado = new Dado();
        Jugadores = new List<Jugador>();
        Turnos = new ColaCircular<Jugador>();
        Turno = 1;
    }

    public void AgregarJugador(Jugador jugador)
    {
        if (jugador is null)
        {
            throw new ArgumentNullException(nameof(jugador));
        }

        Jugadores.Add(jugador);
        Turnos.Agregar(jugador);
    }

    public Jugador? JugadorActual => Turnos.Cantidad > 0 ? Turnos.Actual : null;

    public void AvanzarTurno()
    {
        if (Turnos.Cantidad == 0)
        {
            return;
        }

        Turnos.AvanzarTurno();
        Turno++;
    }

    public void MoverJugadorActual(int pasos)
    {
        Jugador? jugador = JugadorActual;
        if (jugador is null)
        {
            throw new InvalidOperationException("No hay jugadores en la partida.");
        }

        jugador.Mover(pasos, Tablero.Cantidad);
    }
}