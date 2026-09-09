namespace Monopoly.Juego;

public class Dado
{
    private readonly Random random = new();

    public int Lanzar()
    {
        return random.Next(1, 7) + random.Next(1, 7);
    }
}
