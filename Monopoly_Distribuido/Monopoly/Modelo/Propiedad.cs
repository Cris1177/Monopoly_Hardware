namespace Monopoly.Modelos
{
    public class Propiedad : Casilla
    {
        public decimal PrecioCompra { get; set; }
        public decimal Alquiler { get; set; }

        public Jugador? Dueno { get; set; }

        public Propiedad(
            int id,
            string nombre,
            decimal precioCompra,
            decimal alquiler
        ) : base(id, nombre)
        {
            PrecioCompra = precioCompra;
            Alquiler = alquiler;
            Dueno = null;
        }

        public override string ToString()
        {
            return Nombre +
                " | Precio: " +
                PrecioCompra +
                " | Alquiler: " +
                Alquiler;
        }
        public bool EstaDisponible()
        {
            return Dueno == null;
        }
    }
}