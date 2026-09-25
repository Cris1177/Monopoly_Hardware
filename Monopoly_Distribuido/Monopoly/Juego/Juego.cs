using Monopoly.Modelos;
using Monopoly.Estructuras;

namespace Monopoly.Juego
{
    public class Juego
    {
        public Tablero Tablero { get; private set; }

        public Jugador Jugador1 { get; private set; }
        public Jugador Jugador2 { get; private set; }
        public Jugador Jugador3 { get; private set; }
        public Jugador Jugador4 { get; private set; }
        public int NumeroTurno { get; private set; }
        public int MaximoTurnos { get; private set; }
        public Banco Banco { get; private set; }
        
        

        private Dado dado1;
        private Dado dado2;
        private const decimal RECOMPENSA_INICIO = 200m;

        private ColaCircular<Jugador> turnos;
        private ColaCircular<CartaEvento> cartasEvento;
        private bool dadosLanzados;

        public Juego()
        {
            Tablero = new Tablero();

            dado1 = new Dado();
            dado2 = new Dado();

            turnos = new ColaCircular<Jugador>();
            cartasEvento = new ColaCircular<CartaEvento>();
            MaximoTurnos = 50;

            dadosLanzados = false;
            Banco = new Banco();
            

            NumeroTurno = 1;

            CrearJugadores();
            CrearCartasEvento();
        }

        private void CrearJugadores()
        {
            Jugador1 = new Jugador(1, "Christian", 1500m);
            Jugador2 = new Jugador(2, "Isaac", 1500m);
            Jugador3 = new Jugador(3, "Ulfran", 1500m);
            Jugador4 = new Jugador(4, "Fabricio", 1500m);

            Jugador1.Posicion = Tablero.Casillas.Cabeza;
            Jugador2.Posicion = Tablero.Casillas.Cabeza;
            Jugador3.Posicion = Tablero.Casillas.Cabeza;
            Jugador4.Posicion = Tablero.Casillas.Cabeza;

            turnos.Encolar(Jugador1);
            turnos.Encolar(Jugador2);
            turnos.Encolar(Jugador3);
            turnos.Encolar(Jugador4);
        }
        private void CrearCartasEvento()
        {
            cartasEvento.Encolar(
                new CartaEvento(
                    1,
                    "Recibes 500 del banco.",
                    TipoEvento.GanarDinero,
                    500m
                )
            );

            cartasEvento.Encolar(
                new CartaEvento(
                    2,
                    "Debes pagar 300 al banco.",
                    TipoEvento.PagarDinero,
                    300m
                )
            );

            cartasEvento.Encolar(
                new CartaEvento(
                    3,
                    "Avanza 2 casillas.",
                    TipoEvento.Avanzar,
                    2m
                )
            );

            cartasEvento.Encolar(
                new CartaEvento(
                    4,
                    "Retrocede 2 casillas.",
                    TipoEvento.Retroceder,
                    2m
                )
            );

            cartasEvento.Encolar(
                new CartaEvento(
                    5,
                    "Pierdes tu próximo turno.",
                    TipoEvento.PerderTurno,
                    0m
                )
            );

            cartasEvento.Encolar(
                new CartaEvento(
                    6,
                    "Ve directamente a Inicio.",
                    TipoEvento.IrACasilla,
                    0m
                )
            );
        }
        private void BuscarSiguienteJugadorActivo()
        {
            Jugador? jugador = ObtenerJugadorActual();

            int revisados = 0;

            while (
                jugador != null &&
                !jugador.Activo &&
                revisados < 4
            )
            {
                Console.WriteLine(
                    jugador.Nombre +
                    " está eliminado. Se omite su turno."
                );

                turnos.AvanzarTurno();

                jugador = ObtenerJugadorActual();

                revisados++;
            }
        }
        private void EjecutarEvento(Jugador jugador)
        {
            CartaEvento? carta = cartasEvento.ObtenerFrente();

            if (carta == null)
            {
                Console.WriteLine("No hay cartas de evento.");
                return;
            }

            cartasEvento.AvanzarTurno();

            Console.WriteLine();
            Console.WriteLine("= CARTA DE EVENTO =");
            Console.WriteLine(carta.Descripcion);

            if (carta.Tipo == TipoEvento.GanarDinero)
            {
                jugador.Saldo += carta.Valor;

                Banco.RegistrarTransaccion(
                    NumeroTurno,
                    "Ganancia por evento",
                    null,
                    jugador,
                    carta.Valor,
                    carta.Descripcion
                );
            }
            else if (carta.Tipo == TipoEvento.PagarDinero)
            {
                if (jugador.Saldo >= carta.Valor)
                {
                    jugador.Saldo -= carta.Valor;

                    Banco.RegistrarTransaccion(
                        NumeroTurno,
                        "Pago por evento",
                        jugador,
                        null,
                        carta.Valor,
                        carta.Descripcion
                    );
                }
                else
                {
                    Console.WriteLine(
                        jugador.Nombre +
                        " no tiene suficiente dinero para pagar."
                    );

                    EliminarJugador(jugador);
                }
            }
            else if (carta.Tipo == TipoEvento.Avanzar)
            {
                MoverJugador(
                    jugador,
                    (int)carta.Valor
                );

                Console.WriteLine(
                    "Nueva posición: " +
                    jugador.Posicion?.Dato.Nombre
                );

                ProcesarCasilla(jugador);
            }
            else if (carta.Tipo == TipoEvento.Retroceder)
            {
                RetrocederJugador(
                    jugador,
                    (int)carta.Valor
                );

                Console.WriteLine(
                    "Nueva posición: " +
                    jugador.Posicion?.Dato.Nombre
                );
                ProcesarCasilla(jugador);
            }
            else if (carta.Tipo == TipoEvento.PerderTurno)
            {
                jugador.PierdeTurno = true;

                Console.WriteLine(
                    jugador.Nombre +
                    " perderá su próximo turno."
                );
            }
            else if (carta.Tipo == TipoEvento.IrACasilla)
            {
                IrACasilla(
                    jugador,
                    (int)carta.Valor
                );
            }

            Console.WriteLine(
                "Saldo actual de " +
                jugador.Nombre +
                ": " +
                jugador.Saldo
            );

        }
        public void RetrocederJugador(Jugador jugador, int cantidad)
        {
            if (jugador.Posicion == null)
            {
                Console.WriteLine("El jugador no tiene una posición asignada.");
                return;
            }

            for (int i = 0; i < cantidad; i++)
            {
                jugador.Posicion = jugador.Posicion.Anterior;
            }
        }

        //TEMPORALL
             public void MoverYProcesar(Jugador jugador, int cantidad)
        {
            MoverJugador(jugador, cantidad);
            ProcesarCasilla(jugador);
        }

        private void IrACasilla(Jugador jugador, int idCasilla)
{
    Nodo<Casilla>? actual = Tablero.Casillas.Cabeza;

    if (actual == null)
    {
        return;
    }

    do
    {
        if (actual.Dato.Id == idCasilla)
        {
            jugador.Posicion = actual;

            Console.WriteLine(
                jugador.Nombre +
                " fue movido a " +
                actual.Dato.Nombre
            );

            return;
        }

        actual = actual.Siguiente;

    } while (actual != Tablero.Casillas.Cabeza);

    Console.WriteLine(
        "No se encontró la casilla " + idCasilla
    );
}

        public void MoverJugador(Jugador jugador, int cantidad)
        {
            if (jugador.Posicion == null)
            {
                Console.WriteLine("El jugador no tiene una posición asignada.");
                return;
            }

            for (int i = 0; i < cantidad; i++)
            {
                jugador.Posicion = jugador.Posicion.Siguiente;

                if (jugador.Posicion == Tablero.Casillas.Cabeza)
                {
                    jugador.Saldo += RECOMPENSA_INICIO;

                    Banco.RegistrarTransaccion(
                        NumeroTurno,
                        "Recompensa por pasar Inicio",
                        null,
                        jugador,
                        RECOMPENSA_INICIO,
                        "Recompensa por pasar por Inicio"
                    );

                    Console.WriteLine(
                        jugador.Nombre +
                        " pasó por Inicio y recibió " +
                        RECOMPENSA_INICIO
                    );
                }
            }
        }
        private void ProcesarCasilla(Jugador jugador)
        {
            if (jugador.Posicion == null)
            {
                return;
            }

            Casilla casillaActual = jugador.Posicion.Dato;

            Console.WriteLine(
                "Cayó en: " + casillaActual.Nombre
            );

            if (casillaActual is Propiedad propiedad)
            {
                Console.WriteLine("Es una propiedad.");
                Console.WriteLine(
                    "Precio: " + propiedad.PrecioCompra
                );

                Console.WriteLine(
                    "Alquiler: " + propiedad.Alquiler
                );

                if (propiedad.EstaDisponible())
                {
                    Console.WriteLine(
                        "La propiedad está disponible para comprar."
                    );
                }
                else if (propiedad.Dueno == jugador)
                {
                    Console.WriteLine(
                        "Esta propiedad ya pertenece a " +
                        jugador.Nombre + "."
                    );
                }
                else
                {
                    Console.WriteLine(
                        "Esta propiedad pertenece a " +
                        propiedad.Dueno?.Nombre + "."
                    );

                    PagarAlquiler(jugador, propiedad);
                }
            }
            else if (casillaActual is CasillaEvento)
            {
                EjecutarEvento(jugador);
            }
            else if (casillaActual is CasillaEspecial especial)
            {
                Console.WriteLine(
                    "Casilla especial: " +
                    especial.Nombre
                );

                if (especial.Tipo == TipoCasillaEspecial.Inicio)
                {
                    Console.WriteLine(
                        "El jugador está en Salida."
                    );
                }
                else if (especial.Tipo == TipoCasillaEspecial.Impuesto)
                {
                    PagarImpuesto(jugador, especial);
                }
                else if (especial.Tipo == TipoCasillaEspecial.Carcel)
                {
                    Console.WriteLine(
                        "Solo de visita. No ocurre nada."
                    );
                }
                else if (especial.Tipo == TipoCasillaEspecial.ParqueoGratis)
                {
                    Console.WriteLine(
                        "Parque del TEC. No ocurre nada."
                    );
                }
            }
        }

        public void TirarDados()
        {
            if (JuegoTerminado())
            {
                Console.WriteLine(
                    "La partida ya terminó."
                );

                return;
            }
            if (dadosLanzados)
            {
                Console.WriteLine("Ya se lanzaron los dados en este turno.");
                return;
            }

            Jugador? jugadorActual = ObtenerJugadorActual();

            if (jugadorActual == null)
            {
                Console.WriteLine("No hay un jugador actual.");
                return;
            }
            if (!jugadorActual.Activo)
            {
                Console.WriteLine(
                    jugadorActual.Nombre +
                    " está eliminado y no puede lanzar los dados."
                );

                return;
            }

            int resultado1 = dado1.Lanzar();
            int resultado2 = dado2.Lanzar();

            int total = resultado1 + resultado2;

            Console.WriteLine("Dado 1: " + resultado1);
            Console.WriteLine("Dado 2: " + resultado2);
            Console.WriteLine("Total: " + total);

            MoverJugador(jugadorActual, total);

            ProcesarCasilla(jugadorActual);

            dadosLanzados = true;
        }



        public Jugador? ObtenerJugadorActual()
        {
            return turnos.ObtenerFrente();
        }
        public int CantidadJugadoresActivos()
        {
            int cantidad = 0;

            if (Jugador1.Activo)
                cantidad++;

            if (Jugador2.Activo)
                cantidad++;

            if (Jugador3.Activo)
                cantidad++;

            if (Jugador4.Activo)
                cantidad++;

            return cantidad;
        }
        private void PagarImpuesto(
            Jugador jugador,
            CasillaEspecial casilla)
        {
            decimal monto = casilla.Valor;

            if (jugador.Saldo < monto)
            {
                Console.WriteLine(
                    jugador.Nombre +
                    " no tiene suficiente dinero para pagar el impuesto."
                );

                EliminarJugador(jugador);
                return;
            }

            jugador.Saldo -= monto;

            Banco.RegistrarTransaccion(
                NumeroTurno,
                "Pago al banco",
                jugador,
                null,
                monto,
                "Pago de " + casilla.Nombre
            );

            Console.WriteLine(
                jugador.Nombre +
                " pagó " +
                monto +
                " de impuesto."
            );

            Console.WriteLine(
                "Saldo actual: " +
                jugador.Saldo
            );
        }

        public void TerminarTurno()
        {
            if (JuegoTerminado())
            {
                Console.WriteLine(
                    "La partida ya ha terminado."
                );

                return;
            }

            turnos.AvanzarTurno();
            dadosLanzados = false;
            NumeroTurno++;

            int revisados = 0;

            while (revisados < 4)
            {
                Jugador? siguiente =
                    ObtenerJugadorActual();

                if (siguiente == null)
                {
                    return;
                }

                // Jugador eliminado
                if (!siguiente.Activo)
                {
                    Console.WriteLine(
                        siguiente.Nombre +
                        " está eliminado. Se omite su turno."
                    );

                    turnos.AvanzarTurno();
                    NumeroTurno++;
                    revisados++;

                    continue;
                }

                // Jugador que debe perder turno
                if (siguiente.PierdeTurno)
                {
                    Console.WriteLine(
                        siguiente.Nombre +
                        " pierde este turno."
                    );

                    siguiente.PierdeTurno = false;

                    turnos.AvanzarTurno();
                    NumeroTurno++;
                    revisados++;

                    continue;
                }

                // Encontramos un jugador válido.
                break;
            }
            if (JuegoTerminado())
            {
                MostrarResultadoFinal();
            }
        }
        public void ComprarPropiedad()
        {
            if (JuegoTerminado())
            {
                Console.WriteLine(
                    "La partida ya terminó."
                );

                return;
            }
            Jugador? jugador = ObtenerJugadorActual();

            if (jugador == null || jugador.Posicion == null)
            {
                Console.WriteLine("No hay un jugador válido.");
                return;
            }

            if (jugador.Posicion.Dato is not Propiedad propiedad)
            {
                Console.WriteLine("La casilla actual no es una propiedad.");
                return;
            }

            if (!propiedad.EstaDisponible())
            {
                Console.WriteLine("La propiedad ya tiene dueño.");
                return;
            }

            if (jugador.Saldo < propiedad.PrecioCompra)
            {
                Console.WriteLine("No tienes suficiente dinero.");
                return;
            }

            jugador.Saldo -= propiedad.PrecioCompra;

            propiedad.Dueno = jugador;

            jugador.Propiedades.Agregar(propiedad);
            Banco.RegistrarTransaccion(
            NumeroTurno,
            "Compra",
            jugador,
            null,
            propiedad.PrecioCompra,
            "Compra de " + propiedad.Nombre
                );

            Console.WriteLine(
                jugador.Nombre +
                " compró " +
                propiedad.Nombre
            );

            Console.WriteLine(
                "Nuevo saldo: " +
                jugador.Saldo
            );
        }
        private void LiberarPropiedades(
            Jugador jugador)
        {
            Nodo<Propiedad>? actual =
                jugador.Propiedades.ObtenerCabeza();

            while (actual != null)
            {
                actual.Dato.Dueno = null;
                actual = actual.Siguiente;
            }
            jugador.Propiedades.Vaciar();
        }
        private void EliminarJugador(Jugador jugador)
        {
            LiberarPropiedades(jugador);
            jugador.Activo = false;
            jugador.Saldo = 0;

            Console.WriteLine();
            Console.WriteLine(
                jugador.Nombre +
                " ha sido eliminado del juego."
            );
        }
                public bool HayEmpate()
        {
            if (!JuegoTerminado())
            {
                return false;
            }

            decimal mayorPatrimonio = -1m;
            int cantidadConMayor = 0;

            RevisarEmpate(
                Jugador1,
                ref mayorPatrimonio,
                ref cantidadConMayor
            );

            RevisarEmpate(
                Jugador2,
                ref mayorPatrimonio,
                ref cantidadConMayor
            );

            RevisarEmpate(
                Jugador3,
                ref mayorPatrimonio,
                ref cantidadConMayor
            );

            RevisarEmpate(
                Jugador4,
                ref mayorPatrimonio,
                ref cantidadConMayor
            );

            return cantidadConMayor > 1;
        }
        private void RevisarEmpate(
            Jugador jugador,
            ref decimal mayorPatrimonio,
            ref int cantidadConMayor)
        {
            if (!jugador.Activo)
            {
                return;
            }

            decimal patrimonio =
                jugador.CalcularPatrimonio();

            if (patrimonio > mayorPatrimonio)
            {
                mayorPatrimonio = patrimonio;
                cantidadConMayor = 1;
            }
            else if (patrimonio == mayorPatrimonio)
            {
                cantidadConMayor++;
            }
        }

        private void PagarAlquiler(
        Jugador jugador,
        Propiedad propiedad)
        {
            if (propiedad.Dueno == null)
            {
                return;
            }

            if (propiedad.Dueno == jugador)
            {
                Console.WriteLine(
                    "La propiedad pertenece al jugador actual."
                );

                return;
            }

            if (jugador.Saldo < propiedad.Alquiler)
            {
                Console.WriteLine(
                    jugador.Nombre +
                    " no tiene suficiente dinero para pagar el alquiler."
                );

                EliminarJugador(jugador);

                return;
            }

            jugador.Saldo -= propiedad.Alquiler;
            propiedad.Dueno.Saldo += propiedad.Alquiler;

            Banco.RegistrarTransaccion(
                NumeroTurno,
                "Alquiler",
                jugador,
                propiedad.Dueno,
                propiedad.Alquiler,
                "Alquiler de " + propiedad.Nombre
            );

            Console.WriteLine(
                jugador.Nombre +
                " pagó " +
                propiedad.Alquiler +
                " de alquiler a " +
                propiedad.Dueno.Nombre
            );

            Console.WriteLine(
                "Saldo de " +
                jugador.Nombre +
                ": " +
                jugador.Saldo
            );

            Console.WriteLine(
                "Saldo de " +
                propiedad.Dueno.Nombre +
                ": " +
                propiedad.Dueno.Saldo
            );
        }  
        public bool JuegoTerminado()
        {
            if (CantidadJugadoresActivos() <= 1)
            {
                return true;
            }

            if (NumeroTurno > MaximoTurnos)
            {
                return true;
            }

            return false;
        }
       public Jugador? ObtenerGanador()
        {
            if (!JuegoTerminado())
            {
                return null;
            }

            Jugador? ganador = null;

            CompararJugadorParaGanador(
                Jugador1,
                ref ganador
            );

            CompararJugadorParaGanador(
                Jugador2,
                ref ganador
            );

            CompararJugadorParaGanador(
                Jugador3,
                ref ganador
            );

            CompararJugadorParaGanador(
                Jugador4,
                ref ganador
            );

            return ganador;
        }
        private void CompararJugadorParaGanador(
            Jugador jugador,
            ref Jugador? ganador)
        {
            if (!jugador.Activo)
            {
                return;
            }

            if (ganador == null)
            {
                ganador = jugador;
                return;
            }

            if (
                jugador.CalcularPatrimonio() >
                ganador.CalcularPatrimonio()
            )
            {
                ganador = jugador;
            }
        }
        public void MostrarResultadoFinal()
        {
            if (!JuegoTerminado())
            {
                Console.WriteLine(
                    "La partida todavía no ha terminado."
                );

                return;
            }

            Console.WriteLine();
            Console.WriteLine(
                "=============================="
            );

            Console.WriteLine(
                "       FIN DE LA PARTIDA"
            );

            Console.WriteLine(
                "=============================="
            );

            Console.WriteLine();

            MostrarEstadoFinalJugador(Jugador1);
            MostrarEstadoFinalJugador(Jugador2);
            MostrarEstadoFinalJugador(Jugador3);
            MostrarEstadoFinalJugador(Jugador4);

            Console.WriteLine();

            if (HayEmpate())
            {
                Console.WriteLine(
                    "La partida terminó en empate."
                );

                return;
            }

            Jugador? ganador =
                ObtenerGanador();

            if (ganador != null)
            {
                Console.WriteLine(
                    "GANADOR: " +
                    ganador.Nombre
                );

                Console.WriteLine(
                    "Patrimonio: " +
                    ganador.CalcularPatrimonio()
                );
            }
        }
        private void MostrarEstadoFinalJugador(
            Jugador jugador)
        {
            Console.WriteLine(
                jugador.Nombre +
                " | Saldo: " +
                jugador.Saldo +
                " | Propiedades: " +
                jugador.Propiedades.Cantidad +
                " | Patrimonio: " +
                jugador.CalcularPatrimonio() +
                " | Estado: " +
                (jugador.Activo
                    ? "Activo"
                    : "Eliminado")
            );
        }
    }
}