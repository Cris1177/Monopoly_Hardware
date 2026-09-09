namespace Monopoly.Juego;

public class CasillaEspecial : Casilla
{
    public string Efecto { get; }
    public int Monto { get; }

    public CasillaEspecial(string nombre, string efecto, int monto)
        : base(nombre, "Especial")
    {
        Efecto = efecto;
        Monto = monto;
    }

    public override string Ejecutar(Jugador jugador)
    {
        return $"{jugador.Nombre} cayó en {Nombre}: {Efecto}.";
    }
}