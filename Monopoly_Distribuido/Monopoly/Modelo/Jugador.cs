using System.Dynamic;
using Monopoly.Estructuras;
namespace Monopoly.Modelos
{
    public class Jugador
    {
        public int Id { get; set; }
        public string RFIDUid { get; set; } = string.Empty; // Es para idetntificar el llavero RFID
        public string Nombre { get; set; }
        public decimal Saldo { get; set; }
        public bool EnCarcel { get; set; }
        public int TurnosEnCarcel {get; set; }
        public bool Activo { get; set; }

        public bool PierdeTurno{get; set;}

        public Estructuras.Nodo<Casilla>? Posicion { get; set; }
        
        public ListaSimple<Propiedad> Propiedades { get; private set; }
        public Jugador(int id, string nombre, decimal saldoInicial)
        {
            Id = id;
            Nombre = nombre;
            Saldo = saldoInicial;
            Activo = true;
            Propiedades = new ListaSimple<Propiedad>();
            PierdeTurno = false;
            EnCarcel = false;
            TurnosEnCarcel = 0;
            Posicion = null;
        }
        public decimal CalcularPatrimonio()
        {
            decimal total = Saldo;

            Nodo<Propiedad>? actual =
                Propiedades.ObtenerCabeza();

            while (actual != null)
            {
                total += actual.Dato.PrecioCompra;
                actual = actual.Siguiente;
            }

            return total;
        }
    }
}