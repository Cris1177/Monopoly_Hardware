namespace Monopoly.Juego;

public class CasillaEspecial : Casilla
{
    public string Efecto { get; }
    public int Monto { get; }

    public CasillaEspecial(string nombre, string efecto, int monto = 0)
        : base(nombre, "Especial")
    {
        Efecto = efecto;
        Monto = monto;
    }

    public override string Ejecutar(Jugador jugador)
    {
        if (Monto != 0)
        {
            jugador.Recibir(Monto);
        }

        return $"{jugador.Nombre} cayó en {Nombre}: {Efecto}.";
    }
}