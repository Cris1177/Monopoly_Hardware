namespace Monopoly.Juego;

public class Banco
{
    private readonly HistorialTransacciones historial = new();
    private int siguienteIdentificador = 1;

    public HistorialTransacciones Transacciones => historial;

    public void Registrar(Transaccion transaccion)
    {
    }

    public Transaccion CrearTransaccion(
        int turno,
        string tipo,
        string origen,
        string destino,
        int monto,
        string descripcion)
    {
        Transaccion transaccion = new(
            siguienteIdentificador++, turno, tipo, origen, destino, monto, descripcion);
        Registrar(transaccion);
        return transaccion;
    }
}
