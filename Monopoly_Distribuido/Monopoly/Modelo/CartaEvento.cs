namespace Monopoly.Modelos
{
    public class CartaEvento
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public TipoEvento Tipo { get; set; }
        public decimal Valor { get; set; }

        public CartaEvento(
            int id,
            string descripcion,
            TipoEvento tipo,
            decimal valor)
        {
            Id = id;
            Descripcion = descripcion;
            Tipo = tipo;
            Valor = valor;
        }
    }
}