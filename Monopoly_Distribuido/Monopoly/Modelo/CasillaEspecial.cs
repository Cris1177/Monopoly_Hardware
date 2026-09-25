namespace Monopoly.Modelos
{
    public class CasillaEspecial : Casilla
    {
        public TipoCasillaEspecial Tipo { get; set; }
        public decimal Valor { get; set; }

        public CasillaEspecial(
            int id,
            string nombre,
            TipoCasillaEspecial tipo,
            decimal valor = 0m
        ) : base(id, nombre)
        {
            Tipo = tipo;
            Valor = valor;
        }
    }
}