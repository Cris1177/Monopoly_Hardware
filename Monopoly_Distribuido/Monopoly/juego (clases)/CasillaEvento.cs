namespace Monopoly.Juego;

public class CasillaEvento : Casilla
{
    public string Descripcion { get; }
    public int Monto { get; }
    public int Movimiento { get; }

    public CasillaEvento(string nombre, string descripcion, int monto = 0, int movimiento = 0)
        : base(nombre, "Evento")
    {
        Descripcion = descripcion;
        Monto = monto;
        Movimiento = movimiento;
    }

    public override string Ejecutar(Jugador jugador)
    {
        if (Monto != 0)
        {
            jugador.Recibir(Monto);
        }

        if (Movimiento != 0)
        {
            jugador.Mover(Movimiento, 24);
        }

        return $"{jugador.Nombre}: {Descripcion} ({Monto:+#;-#;0}).";
    }
}