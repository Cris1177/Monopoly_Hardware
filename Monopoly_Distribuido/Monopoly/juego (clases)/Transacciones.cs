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
}
