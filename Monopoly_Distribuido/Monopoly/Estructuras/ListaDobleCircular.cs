using Monopoly.Modelos;

namespace Monopoly.Estructuras
{
    public class ListaDobleCircular
    {
        public Nodo<Casilla> Cabeza { get; private set; }

        public int Cantidad { get; private set; }

        public ListaDobleCircular()
        {
            Cabeza = null;
            Cantidad = 0;
        }

        public void Agregar(Casilla casilla)
        {
            Nodo<Casilla> nuevo = new Nodo<Casilla>(casilla);

            if (Cabeza == null)
            {
                Cabeza = nuevo;

                nuevo.Siguiente = nuevo;
                nuevo.Anterior = nuevo;
            }
            else
            {
                Nodo<Casilla> ultimo = Cabeza.Anterior;

                nuevo.Siguiente = Cabeza;
                nuevo.Anterior = ultimo;

                ultimo.Siguiente = nuevo;
                Cabeza.Anterior = nuevo;
            }

            Cantidad++;
        }
    }
}