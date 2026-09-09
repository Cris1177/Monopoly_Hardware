namespace Monopoly.Juego;

public class Propiedad : Casilla
{
    public int Identificador { get; }
    public int Precio { get; }
    public int Alquiler { get; }
    public Jugador? Dueño { get; private set; }

    public Propiedad(int identificador, string nombre, int precio, int alquiler)
        : base(nombre, "Propiedad")
    {
        Identificador = identificador;
        Precio = precio;
        Alquiler = alquiler;
    }

    public Propiedad(string nombre, int precio, int alquiler)
        : this(0, nombre, precio, alquiler)
    {
    }

    public override string Ejecutar(Jugador jugador)
    {
        if (Dueño is null)
        {
            return $"{jugador.Nombre} puede comprar {Nombre} por {Precio}.";
        }

        if (Dueño == jugador)
        {
            return $"{jugador.Nombre} ya es dueño de {Nombre}.";
        }

        if (jugador.Dinero < Alquiler)
        {
            jugador.Estado = false;
            return $"{jugador.Nombre} no puede pagar el alquiler de {Nombre}.";
        }

        jugador.Dinero -= Alquiler;
        Dueño.Dinero += Alquiler;
        return $"{jugador.Nombre} pagó {Alquiler} de alquiler a {Dueño.Nombre}.";
    }

    public bool Comprar(Jugador jugador)
    {
        if (Dueño is not null || jugador.Dinero < Precio)
        {
            return false;
        }

        jugador.Dinero -= Precio;
        Dueño = jugador;
        return true;
    }
}
