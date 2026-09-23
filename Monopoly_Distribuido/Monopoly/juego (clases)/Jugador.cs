namespace Monopoly.Juego;

public class Jugador
{
    public int Identificador { get; }
    public string Nombre { get; }
    public int Saldo { get; set; }
    public int PosicionActual { get; set; }
    public bool EstadoActivo { get; set; }
    public List<Propiedad> PropiedadesAdquiridas { get; }

    public int Dinero
    {
        get => Saldo;
        set => Saldo = value;
    }

    public int Posicion
    {
        get => PosicionActual;
        set => PosicionActual = value;
    }

    public bool Estado
    {
        get => EstadoActivo;
        set => EstadoActivo = value;
    }

    public int identificacion
    {
        get => Identificador;
        set => throw new InvalidOperationException("El identificador del jugador no puede modificarse.");
    }

    public Jugador(string nombre, int identificador)
    {
        Nombre = nombre;
        Identificador = identificador;
        Saldo = 1500;
        PosicionActual = 0;
        EstadoActivo = true;
        PropiedadesAdquiridas = new List<Propiedad>();
    }

    public void Mover(int pasos, int totalCasillas)
    {
        if (totalCasillas <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalCasillas));
        }

        int nuevaPosicion = (PosicionActual + pasos) % totalCasillas;
        if (nuevaPosicion < 0)
        {
            nuevaPosicion += totalCasillas;
        }

        PosicionActual = nuevaPosicion;
    }

    public void AgregarPropiedad(Propiedad propiedad)
    {
        if (propiedad is null)
        {
            throw new ArgumentNullException(nameof(propiedad));
        }

        if (!PropiedadesAdquiridas.Contains(propiedad))
        {
            PropiedadesAdquiridas.Add(propiedad);
        }
    }

    public bool PuedePagar(int monto)
    {
        return Saldo >= monto;
    }

    public void Pagar(int monto)
    {
        if (monto < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monto));
        }

        Saldo -= monto;
    }

    public void Recibir(int monto)
    {
        if (monto < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(monto));
        }

        Saldo += monto;
    }

    public int Patrimonio()
    {
        int total = Saldo;
        foreach (Propiedad propiedad in PropiedadesAdquiridas)
        {
            total += propiedad.PrecioCompra;
        }

        return total;
    }

    public bool EstaEnBancarrota()
    {
        return Saldo < 0;
    }
}