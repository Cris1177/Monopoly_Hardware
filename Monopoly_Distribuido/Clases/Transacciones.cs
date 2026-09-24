namespace Monopoly.Juego;

public class Transaccion
{
    public int Identificador { get; }
    public DateTime FechaHora { get; }
    public int NumeroTurno { get; }
    public string Tipo { get; }
    public string Origen { get; }
    public string Destino { get; }
    public int Monto { get; }
    public string Descripcion { get; }

    public Transaccion(
        int identificador,
        int numeroTurno,
        string tipo,
        string origen,
        string destino,
        int monto,
        string descripcion)
    {
        Identificador = identificador;
        FechaHora = DateTime.Now;
        NumeroTurno = numeroTurno;
        Tipo = tipo;
        Origen = origen;
        Destino = destino;
        Monto = monto;
        Descripcion = descripcion;
    }
}

public class HistorialTransacciones
{
    private readonly LinkedList<Transaccion> transacciones = new();

    public int Count => transacciones.Count;

    public void Agregar(Transaccion transaccion)
    {
        if (transaccion is null)
        {
            throw new ArgumentNullException(nameof(transaccion));
        }

        transacciones.AddLast(transaccion);
    }

    public IEnumerable<Transaccion> DesdeMasAntigua()
    {
        foreach (Transaccion transaccion in transacciones)
        {
            yield return transaccion;
        }
    }

    public IEnumerable<Transaccion> DesdeMasReciente()
    {
        LinkedListNode<Transaccion>? actual = transacciones.Last;
        while (actual is not null)
        {
            yield return actual.Value;
            actual = actual.Previous;
        }
    }

    public IEnumerable<Transaccion> BuscarPorJugador(string nombreJugador)
    {
        foreach (Transaccion transaccion in transacciones)
        {
            if (string.Equals(transaccion.Origen, nombreJugador, StringComparison.OrdinalIgnoreCase)
                || string.Equals(transaccion.Destino, nombreJugador, StringComparison.OrdinalIgnoreCase))
            {
                yield return transaccion;
            }
        }
    }

    public IEnumerable<Transaccion> BuscarPorTipo(string tipo)
    {
        foreach (Transaccion transaccion in transacciones)
        {
            if (string.Equals(transaccion.Tipo, tipo, StringComparison.OrdinalIgnoreCase))
            {
                yield return transaccion;
            }
        }
    }

    public IEnumerable<Transaccion> ObtenerTodas()
    {
        return DesdeMasAntigua();
    }

    public void ImprimirTodas(TextWriter? escritor = null)
    {
        escritor ??= Console.Out;
        foreach (Transaccion transaccion in transacciones)
        {
            escritor.WriteLine($"#{transaccion.Identificador} | Turno {transaccion.NumeroTurno} | {transaccion.Tipo} | Origen: {transaccion.Origen} | Destino: {transaccion.Destino} | Monto: {transaccion.Monto} | {transaccion.Descripcion}");
        }
    }

    public void ExportarTxt(string rutaArchivo)
    {
        using StreamWriter escritor = new(rutaArchivo);
        escritor.WriteLine("#Transaccion|Turno|Tipo|Origen|Destino|Monto|Descripcion");
        foreach (Transaccion transaccion in transacciones)
        {
            escritor.WriteLine($"{transaccion.Identificador}|{transaccion.NumeroTurno}|{transaccion.Tipo}|{transaccion.Origen}|{transaccion.Destino}|{transaccion.Monto}|{transaccion.Descripcion}");
        }
    }
}
