namespace Monopoly.Juego;

public class Jugador
{
    public string Nombre { get; set; }
    public int identificacion { get; set; }
    public int Dinero { get; set; }
    public int Posicion { get; set; }
    public bool Estado { get; set; }

    public Jugador(string nombre, int id)
    {
        identificacion = id;
        Nombre = nombre;
        Dinero = 1500;
        Posicion = 0;
        Estado = true;
    }
    public void Mover(int pasos, int totalCasillas)
    {
        if (totalCasillas <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalCasillas));
        }

        Posicion = (Posicion + pasos) % totalCasillas;
        if (Posicion < 0)
        {
            Posicion += totalCasillas;
        }
    }

    public int Patrimonio()
    {
        return Dinero;
    }

    public bool EstaEnBancarrota()
    {
        return Dinero < 0;
    }
}