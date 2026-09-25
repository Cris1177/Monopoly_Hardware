namespace Monopoly.Modelos
{
    public class Transaccion
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public int NumeroTurno { get; set; }
        public string Tipo { get; set; }

        public Jugador? Origen { get; set; }
        public Jugador? Destino { get; set; }

        public decimal Monto { get; set; }
        public string Descripcion { get; set; }

        public Transaccion(
            int id,
            int numeroTurno,
            string tipo,
            Jugador? origen,
            Jugador? destino,
            decimal monto,
            string descripcion)
        {
            Id = id;
            FechaHora = DateTime.Now;
            NumeroTurno = numeroTurno;
            Tipo = tipo;
            Origen = origen;
            Destino = destino;
            Monto = monto;
            Descripcion = descripcion;
        }
    }
}