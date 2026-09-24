namespace Monopoly.Juego;

public class CartaEvento
{
    public string Descripcion { get; }
    public int Monto { get; }
    public int Movimiento { get; }
    public string Tipo { get; }

    public CartaEvento(string descripcion, int monto = 0, int movimiento = 0, string? tipo = null)
    {
        Descripcion = descripcion;
        Monto = monto;
        Movimiento = movimiento;
        Tipo = tipo ?? (monto > 0 ? "Recibir dinero" : monto < 0 ? "Pagar dinero" : movimiento != 0 ? "Movimiento" : "Evento");
    }

    public string Aplicar(Jugador jugador, int totalCasillas)
    {
        if (Monto != 0)
        {
            jugador.Recibir(Monto);
        }

        if (Movimiento != 0)
        {
            jugador.Mover(Movimiento, totalCasillas);
        }

        return $"{jugador.Nombre}: {Descripcion}.";
    }
}
