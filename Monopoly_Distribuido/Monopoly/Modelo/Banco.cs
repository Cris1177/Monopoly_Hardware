using Monopoly.Estructuras;
using System.IO;

namespace Monopoly.Modelos
{
    public class Banco
    {
        public ListaSimple<Transaccion> Historial { get; private set; }
        private int siguienteId;

        public Banco()
        {
            Historial = new ListaSimple<Transaccion>();
            siguienteId = 1;
        }

        public void RegistrarTransaccion(
            int numeroTurno,
            string tipo,
            Jugador? origen,
            Jugador? destino,
            decimal monto,
            string descripcion)
        {
            Transaccion transaccion = new Transaccion(
                siguienteId,
                numeroTurno,
                tipo,
                origen,
                destino,
                monto,
                descripcion
            );

            Historial.Agregar(transaccion);
            siguienteId++;
        }

        public void MostrarHistorial()
        {
            Nodo<Transaccion>? actual =
                Historial.ObtenerCabeza();

            Console.WriteLine();
            Console.WriteLine(
                "=== HISTORIAL DE TRANSACCIONES ==="
            );

            if (actual == null)
            {
                Console.WriteLine(
                    "No hay transacciones registradas."
                );

                return;
            }

            while (actual != null)
            {
                MostrarTransaccion(actual.Dato);
                actual = actual.Siguiente;
            }
        }

        private void MostrarTransaccion(Transaccion t)
        {
            Console.WriteLine();

            Console.WriteLine("ID: " + t.Id);
            Console.WriteLine("Fecha: " + t.FechaHora);
            Console.WriteLine("Turno: " + t.NumeroTurno);
            Console.WriteLine("Tipo: " + t.Tipo);

            Console.WriteLine(
                "Origen: " +
                (t.Origen?.Nombre ?? "Banco")
            );

            Console.WriteLine(
                "Destino: " +
                (t.Destino?.Nombre ?? "Banco")
            );

            Console.WriteLine("Monto: " + t.Monto);

            Console.WriteLine(
                "Descripción: " +
                t.Descripcion
            );
        }

        public void BuscarPorJugador(Jugador jugador)
        {
            Nodo<Transaccion>? actual =
                Historial.ObtenerCabeza();

            Console.WriteLine();
            Console.WriteLine(
                "=== TRANSACCIONES DE " +
                jugador.Nombre.ToUpper() +
                " ==="
            );

            bool encontrado = false;

            while (actual != null)
            {
                Transaccion transaccion =
                    actual.Dato;

                if (
                    transaccion.Origen == jugador ||
                    transaccion.Destino == jugador
                )
                {
                    MostrarTransaccion(
                        transaccion
                    );

                    encontrado = true;
                }

                actual = actual.Siguiente;
            }

            if (!encontrado)
            {
                Console.WriteLine(
                    "No se encontraron transacciones."
                );
            }
        }

        public void BuscarPorTipo(string tipo)
        {
            Nodo<Transaccion>? actual =
                Historial.ObtenerCabeza();

            Console.WriteLine();
            Console.WriteLine(
                "=== TRANSACCIONES: " +
                tipo.ToUpper() +
                " ==="
            );

            bool encontrado = false;

            while (actual != null)
            {
                Transaccion transaccion =
                    actual.Dato;

                if (
                    transaccion.Tipo.Equals(
                        tipo,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    MostrarTransaccion(
                        transaccion
                    );

                    encontrado = true;
                }

                actual = actual.Siguiente;
            }

            if (!encontrado)
            {
                Console.WriteLine(
                    "No se encontraron transacciones."
                );
            }
        }

        public void MostrarHistorialReciente()
        {
            Console.WriteLine();
            Console.WriteLine(
                "=== HISTORIAL MÁS RECIENTE → MÁS ANTIGUO ==="
            );

            Nodo<Transaccion>? cabeza =
                Historial.ObtenerCabeza();

            if (cabeza == null)
            {
                Console.WriteLine(
                    "No hay transacciones registradas."
                );

                return;
            }

            MostrarInverso(cabeza);
        }

        // ESTE ES EL MÉTODO QUE TE ESTABA FALTANDO
        private void MostrarInverso(
            Nodo<Transaccion>? nodo)
        {
            if (nodo == null)
            {
                return;
            }

            MostrarInverso(
                nodo.Siguiente
            );

            MostrarTransaccion(
                nodo.Dato
            );
        }

        public void ExportarHistorial(
            string nombreArchivo)
        {
            using StreamWriter archivo =
                new StreamWriter(nombreArchivo);

            Nodo<Transaccion>? actual =
                Historial.ObtenerCabeza();

            archivo.WriteLine(
                "=== HISTORIAL DE TRANSACCIONES ==="
            );

            archivo.WriteLine();

            while (actual != null)
            {
                Transaccion t = actual.Dato;

                archivo.WriteLine(
                    "ID: " + t.Id
                );

                archivo.WriteLine(
                    "Fecha: " + t.FechaHora
                );

                archivo.WriteLine(
                    "Turno: " + t.NumeroTurno
                );

                archivo.WriteLine(
                    "Tipo: " + t.Tipo
                );

                archivo.WriteLine(
                    "Origen: " +
                    (t.Origen?.Nombre ?? "Banco")
                );

                archivo.WriteLine(
                    "Destino: " +
                    (t.Destino?.Nombre ?? "Banco")
                );

                archivo.WriteLine(
                    "Monto: " + t.Monto
                );

                archivo.WriteLine(
                    "Descripción: " +
                    t.Descripcion
                );

                archivo.WriteLine(
                    "--------------------------------"
                );

                actual = actual.Siguiente;
            }

            Console.WriteLine(
                "Historial exportado en: " +
                Path.GetFullPath(nombreArchivo)
            );
        }
    }
}