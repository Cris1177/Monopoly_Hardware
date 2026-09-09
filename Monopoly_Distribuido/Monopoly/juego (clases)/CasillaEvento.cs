namespace Monopoly.Juego;

public class CasillaEvento : Casilla
{
    public string Descripcion { get; }
    public int Monto { get; }

    public CasillaEvento(string nombre, string descripcion, int monto)
        : base(nombre, "Evento")
    {
        Descripcion = descripcion;
        Monto = monto;
    }

    public override string Ejecutar(Jugador jugador)
    {
        jugador.Dinero += Monto;
        return $"{jugador.Nombre}: {Descripcion} ({Monto:+#;-#;0}).";
    }
}