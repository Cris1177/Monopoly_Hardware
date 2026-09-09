namespace Monopoly.Juego;

public class Juego
{
    public Tablero Tablero { get; }
    public Banco Banco { get; }
    public Dado Dado { get; }
    public int Turno { get; private set; }

    public Juego()
    {
        Tablero = new Tablero();
        Banco = new Banco();
        Dado = new Dado();
    }

    public void AvanzarTurno()
    {
        Turno++;
    }

}