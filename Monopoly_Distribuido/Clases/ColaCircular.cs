namespace Monopoly.Juego;

// Cola Circular pensada para rotar el turno entre los jugadores. 
// Un puntero se va moviendo al siguiente nodo en cada turno

public class ColaCircular<T>
{
    private class Nodo
    {
        public T Valor;
        public Nodo? Siguiente;

        public Nodo (T Valor)
        {
            Valor = valor;
            Siguiente = null;
        }
    }

    private Nodo? actual;

    public int Cantidad {get; private set; }


    // Agrega un elemento nuevo al circulo justo antes del que esta como actual (al final de la vuelta)

    public void Agregar(T valor)
    {
        Nodo nuevo = new(Valor);

        if (actual is null)
        {
            // Primer elemento: apunta a si mismo para cerrar el circulo.

            nuevo.Siguiente = nuevo;
            actual = nuevo;
        }
        else
        {
            Nodo ultimo = actual;
            while (ultimo.Siguiente != actual)
            {
                ultimo = ultimo.Siguiente!;
            }

            ultimo.Siguiente = nuevo;
            nuevo.Siguiente = actual;
        }
        Cantidad++;
    }
    // El jugador que tiene el turno en este momento
    public T Actual
    {
        get
        {
            if (actual is null)
            {
                throw new InvalidOperationException("La cola de turnos está vacía");
            }
            return actual.Valor;
        }
    }

    public void AvanzarTurno()
    {
        if (actual is null)
        {
            return;
        }
        actual = actual.Siguiente;
    }

    // Sacar al jugador cuando quede en bancarrota

    public void EliminarActual()
    {
        if (actual is null)
        {
            return;
        }

        if (actual.Siguiente == actual)
        {
            actual = null; // Unico jugador en la cola
            Cantidad = 0;
            return;
        }

        Nodo predecesor = actual;
        while (predecesor.Siguiente != actual)
        {
            predecesor = predecesor.Siguiente!;
        }

        predecesor.Siguiente = actual.Siguiente;
        actual = actual.Siguiente;
        Cantidad--;
    }
}
