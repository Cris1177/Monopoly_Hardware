namespace Monopoly.Juego;

public class Dado
{
    private readonly Random random = new();
    public int UltimoResultado { get; private set; }

    public int Lanzar()
    {
        int primerDado = random.Next(1, 7);
        int segundoDado = random.Next(1, 7);
        UltimoResultado = primerDado + segundoDado;
        return UltimoResultado;
    }

    public (int Primero, int Segundo, int Total) LanzarDados()
    {
        int primero = random.Next(1, 7);
        int segundo = random.Next(1, 7);
        UltimoResultado = primero + segundo;
        return (primero, segundo, UltimoResultado);
    }
}
