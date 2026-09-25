namespace Monopoly.Estructuras
{
    public class ListaDoble<T>
    {
        public Nodo<T>? Cabeza { get; private set; }
        public Nodo<T>? Cola { get; private set; }
        public int Cantidad { get; private set; }

        public ListaDoble()
        {
            Cabeza = null;
            Cola = null;
            Cantidad = 0;
        }

        public void Agregar(T dato)
        {
            Nodo<T> nuevo = new Nodo<T>(dato);

            if (Cabeza == null)
            {
                Cabeza = nuevo;
                Cola = nuevo;
            }
            else
            {
                Cola!.Siguiente = nuevo;
                nuevo.Anterior = Cola;
                Cola = nuevo;
            }

            Cantidad++;
        }
    }
}