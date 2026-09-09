namespace Monopoly.Juego;

public class CartaEvento
{
    public string Descripcion { get; }
    public int Monto { get; }
    public int Movimiento { get; }

    public CartaEvento(string descripcion, int monto = 0, int movimiento = 0)
    {
        Descripcion = descripcion;
        Monto = monto;
        Movimiento = movimiento;
    }

    public string Aplicar(Jugador jugador, int totalCasillas)
    {
        jugador.Dinero += Monto;
        jugador.Mover(Movimiento, totalCasillas);
        return $"{jugador.Nombre}: {Descripcion}.";
    }
}
