namespace Monopoly.Estructuras
{
    public class ListaSimple<T>
    {
        private Nodo<T>? cabeza;

        public int Cantidad { get; private set; }

        public ListaSimple()
        {
            cabeza = null;
            Cantidad = 0;
        }
        public void Vaciar()
        {
            cabeza = null;
            Cantidad = 0;
        }

        public void Agregar(T dato)
        {
            Nodo<T> nuevo = new Nodo<T>(dato);

            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                Nodo<T> actual = cabeza;

                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }

                actual.Siguiente = nuevo;
            }

            Cantidad++;
        }

        public void Mostrar()
        {
            Nodo<T>? actual = cabeza;

            while (actual != null)
            {
                Console.WriteLine(actual.Dato);
                actual = actual.Siguiente;
            }
        }
        public Nodo<T>? ObtenerCabeza()
        {
            return cabeza;
        }
    }
}