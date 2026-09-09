namespace Monopoly.Juego;

public abstract class Casilla
{
    public string Nombre { get; }
    public string Tipo { get; }
    

    protected Casilla(string nombre, string tipo)
{
        Nombre = nombre;
        Tipo = tipo;
    }

    public abstract string Ejecutar(Jugador jugador);
}
