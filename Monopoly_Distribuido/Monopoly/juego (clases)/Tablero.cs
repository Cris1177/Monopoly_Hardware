namespace Monopoly.Juego;

public class Tablero
{
    public sealed class Nodo
    {
        public Casilla Casilla { get; }
        public Nodo Anterior { get; set; }
        public Nodo Siguiente { get; set; }

        public Nodo(Casilla casilla)
        {
            Casilla = casilla;
            Anterior = this;
            Siguiente = this;
        }
    }

    public Nodo? Primero { get; private set; }
    public int Cantidad { get; private set; }

    public void Agregar(Casilla casilla)
    {
        Nodo nuevo = new(casilla);
        if (Primero is null)
        {
            Primero = nuevo;
        }
        else
        {
            Nodo ultimo = Primero.Anterior;
            nuevo.Anterior = ultimo;
            nuevo.Siguiente = Primero;
            ultimo.Siguiente = nuevo;
            Primero.Anterior = nuevo;
        }

        Cantidad++;
    }

    public Nodo Obtener(int posicion)
    {
        if (Primero is null || posicion < 0 || posicion >= Cantidad)
        {
            throw new ArgumentOutOfRangeException(nameof(posicion));
        }

        Nodo actual = Primero;
        for (int indice = 0; indice < posicion; indice++)
        {
            actual = actual.Siguiente;
        }

        return actual;
    }
}
