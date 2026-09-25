namespace Monopoly.Juego;

public class Propiedad : Casilla
{
    public int Identificador { get; }
    public int PrecioCompra { get; }
    public int Alquiler { get; }
    public Jugador? Propietario { get; private set; }

    public int Precio
    {
        get => PrecioCompra;
    }

    public Jugador? Dueño
    {
        get => Propietario;
        private set => Propietario = value;
    }

    public Propiedad(int identificador, string nombre, int precioCompra, int alquiler)
        : base(nombre, "Propiedad")
    {
        Identificador = identificador;
        PrecioCompra = precioCompra;
        Alquiler = alquiler;
    }

    public Propiedad(string nombre, int precioCompra, int alquiler)
        : this(0, nombre, precioCompra, alquiler)
    {
    }

    public override string Ejecutar(Jugador jugador)
    {
        if (Propietario is null)
        {
            return $"{jugador.Nombre} cayó en {Nombre}. Está disponible por {PrecioCompra}.";
        }

        if (Propietario == jugador)
        {
            return $"{jugador.Nombre} ya es dueño de {Nombre}.";
        }

        if (!jugador.PuedePagar(Alquiler))
        {
            jugador.EstadoActivo = false;
            return $"{jugador.Nombre} no puede pagar el alquiler de {Nombre}.";
        }

        jugador.Pagar(Alquiler);
        Propietario.Recibir(Alquiler);
        return $"{jugador.Nombre} pagó {Alquiler} de alquiler a {Propietario.Nombre}.";
    }

    public bool Comprar(Jugador jugador)
    {
        if (Propietario is not null || !jugador.PuedePagar(PrecioCompra))
        {
            return false;
        }

        jugador.Pagar(PrecioCompra);
        Propietario = jugador;
        jugador.AgregarPropiedad(this);
        return true;
    }
}
