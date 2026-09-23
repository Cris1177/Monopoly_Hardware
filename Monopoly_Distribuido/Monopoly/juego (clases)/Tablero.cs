namespace Monopoly.Juego;

public class Tablero
{
    public sealed class Nodo
    {
        public Casilla Casilla { get; }
        public Nodo? Anterior { get; set; }
        public Nodo? Siguiente { get; set; }

        public Nodo(Casilla casilla)
        {
            Casilla = casilla;
            Anterior = null;
            Siguiente = null;
        }
    }

    public Nodo? Primero { get; private set; }
    public int Cantidad { get; private set; }

    public Tablero()
    {
        CrearTableroBase();
    }

    public void CrearTableroBase()
    {
        for (int indice = 0; indice < 24; indice++)
        {
            Agregar(new CasillaEspecial($"Casilla {indice + 1}", "Sin efecto especial"));
        }
    }

    public void Agregar(Casilla casilla)
    {
        if (casilla is null)
        {
            throw new ArgumentNullException(nameof(casilla));
        }

        Nodo nuevo = new(casilla);
        if (Primero is null)
        {
            Primero = nuevo;
            nuevo.Anterior = nuevo;
            nuevo.Siguiente = nuevo;
            Cantidad = 1;
            return;
        }

        Nodo ultimo = Primero.Anterior!;
        nuevo.Anterior = ultimo;
        nuevo.Siguiente = Primero;
        ultimo.Siguiente = nuevo;
        Primero.Anterior = nuevo;
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
            actual = actual.Siguiente!;
        }

        return actual;
    }

    public Nodo ObtenerNodoPorJugador(Jugador jugador)
    {
        if (Primero is null)
        {
            throw new InvalidOperationException("El tablero aún no tiene casillas.");
        }

        Nodo actual = Primero;
        for (int indice = 0; indice < Cantidad; indice++)
        {
            if (indice == jugador.PosicionActual)
            {
                return actual;
            }

            actual = actual.Siguiente!;
        }

        throw new InvalidOperationException("La posición del jugador no existe en el tablero.");
    }
}
