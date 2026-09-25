namespace Monopoly.Estructuras
{
    public class ColaCircular<T>
    {
        private Nodo<T>? frente;
        private Nodo<T>? ultimo;

        public int Cantidad { get; private set; }

        public ColaCircular()
        {
            frente = null;
            ultimo = null;
            Cantidad = 0;
        }

        public bool EstaVacia()
        {
            return frente == null;
        }

        public void Encolar(T dato)
        {
            Nodo<T> nuevo = new Nodo<T>(dato);

            if (EstaVacia())
            {
                frente = nuevo;
                ultimo = nuevo;

                nuevo.Siguiente = nuevo;
                nuevo.Anterior = nuevo;
            }
            else
            {
                nuevo.Siguiente = frente;
                nuevo.Anterior = ultimo;

                ultimo!.Siguiente = nuevo;
                frente!.Anterior = nuevo;

                ultimo = nuevo;
            }

            Cantidad++;
        }

        public T? ObtenerFrente()
        {
            if (EstaVacia())
            {
                return default;
            }

            return frente!.Dato;
        }

        public void AvanzarTurno()
        {
            if (!EstaVacia())
            {
                frente = frente!.Siguiente;
                ultimo = ultimo!.Siguiente;
            }
        }
    }
}