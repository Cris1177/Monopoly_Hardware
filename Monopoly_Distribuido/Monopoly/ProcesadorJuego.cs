using System;
using System.Text.Json;
using Monopoly.Juego;
using Monopoly.Modelos;
using Monopoly.Protocolo;

namespace Monopoly.Comunicacion
{
    public class ProcesadorJuego : IProcesadorAcciones
    {
        private readonly Juego.Juego _juego;

        public Jugador? ObtenerJugadorActual()
        {
            return _juego.ObtenerJugadorActual();
        }

        public ProcesadorJuego(Juego.Juego juego)
        {
            _juego = juego;
        }

        public Mensaje Procesar(Mensaje solicitud)
        {
            // Validar que el jugador que envía la acción sea el del turno activo
            // (salvo para la acción de CONECTAR o consultas de estado)
            if (solicitud.Accion != TipoAccion.CONECTAR &&
                solicitud.Accion != TipoAccion.CONSULTAR_ESTADO &&
                solicitud.Accion != TipoAccion.CONSULTAR_TRANSACCIONES)
            {
                Jugador? actual = _juego.ObtenerJugadorActual();
                if (actual == null || actual.Id != solicitud.IdJugador)
                {
                    return CrearRespuesta(solicitud, false, $"No es el turno del jugador {solicitud.IdJugador}. Es el turno de {actual?.Nombre}.");
                }
            }

            return solicitud.Accion switch
            {
                TipoAccion.CONECTAR => ProcesarConectar(solicitud),
                TipoAccion.TIRAR_DADOS => ProcesarTirarDados(solicitud),
                TipoAccion.COMPRAR_PROPIEDAD => ProcesarComprarPropiedad(solicitud),
                TipoAccion.NO_COMPRAR => ProcesarNoComprar(solicitud),
                TipoAccion.PAGAR => ProcesarPagar(solicitud),
                TipoAccion.TERMINAR_TURNO => ProcesarTerminarTurno(solicitud),
                TipoAccion.CONSULTAR_ESTADO => ProcesarConsultarEstado(solicitud),
                TipoAccion.CONSULTAR_TRANSACCIONES => ProcesarConsultarTransacciones(solicitud),
                _ => CrearRespuesta(solicitud, false, "Acción no reconocida o no soportada.")
            };
        }

        private Mensaje ProcesarConectar(Mensaje solicitud)
        {
            Console.WriteLine($"[Procesador] Jugador {solicitud.IdJugador} conectado exitosamente.");
            return CrearRespuesta(solicitud, true, $"Jugador {solicitud.IdJugador} bienvenido a la partida.");
        }

        private Mensaje ProcesarTirarDados(Mensaje solicitud)
        {       
            if (_juego.JuegoTerminado())
            {
                return CrearRespuesta(solicitud, false, "La partida ya terminó.");
            }

            Jugador? jugador = _juego.ObtenerJugadorActual();
            if (jugador == null || !jugador.Activo)
            {
                return CrearRespuesta(solicitud, false, "No hay un jugador activo para este turno.");
            }

            // Regla del enunciado: un solo lanzamiento de dados por turno
            if (_juego.DadosLanzados)
            {
                return CrearRespuesta(solicitud, false, "Ya se lanzaron los dados en este turno.");
            }

            int valorDado = 0;

            // 1. Obtener el valor del dado desde la Pico
            if (solicitud.Datos.HasValue && solicitud.Datos.Value.TryGetProperty("valor", out var valorProp))
            {
                valorDado = valorProp.GetInt32();
            }
            else if (int.TryParse(solicitud.Descripcion, out int valorParsed))
            {
                valorDado = valorParsed;
            }

            if (valorDado > 0)
            {
                // Movemos al jugador y procesamos la casilla
                _juego.TirarDados(valorDado);

                // PASO 3: Evaluar si cayó en una propiedad comprable para pedir confirmación por RFID
                if (jugador.Posicion?.Dato is Propiedad propiedad && propiedad.EstaDisponible())
                {
                    Console.WriteLine();
                    Console.WriteLine("=================================================");
                    Console.WriteLine($" ¿DESEAS COMPRAR {propiedad.Nombre.ToUpper()}?");
                    Console.WriteLine($" Precio: {propiedad.PrecioCompra} | Saldo de {jugador.Nombre}: {jugador.Saldo}");
                    Console.WriteLine(" --> ACERCA TU TARJETA/LLAVERO RFID AL LECTOR PARA COMPRAR <--");
                    Console.WriteLine("=================================================");
                }

                return CrearRespuesta(solicitud, true, $"{jugador.Nombre} avanzó {valorDado} casillas.");
            }

            // Fallback si no viene valor físico
            _juego.TirarDados();
            return CrearRespuesta(solicitud, true, "Dados lanzados exitosamente.");
        }

        private Mensaje ProcesarComprarPropiedad(Mensaje solicitud)
        {
            Jugador? jugador = _juego.ObtenerJugadorActual();
            if (jugador == null || jugador.Posicion == null)
            {
                return CrearRespuesta(solicitud, false, "Estado de jugador inválido.");
            }

            if (jugador.Posicion.Dato is not Propiedad propiedad)
            {
                return CrearRespuesta(solicitud, false, "La casilla actual no es una propiedad comprable.");
            }

            if (!propiedad.EstaDisponible())
            {
                return CrearRespuesta(solicitud, false, "La propiedad no está disponible.");
            }

            if (jugador.Saldo < propiedad.PrecioCompra)
            {
                Console.WriteLine($"[RFID] {jugador.Nombre} intentó comprar pero no tiene saldo suficiente.");
                return CrearRespuesta(solicitud, false, "Saldo insuficiente para comprar la propiedad.");
            }

            // Obtener UID leído por el sensor RFID (si viene en Descripcion o Datos)
            string uidRfid = solicitud.Descripcion ?? "";

            // Efectuar la compra en el juego
            _juego.ComprarPropiedad();

            Console.WriteLine($"\n[COMPRA EXITOSA] Propiedad '{propiedad.Nombre}' adquirida mediante RFID ({uidRfid}).");
            return CrearRespuesta(solicitud, true, $"{jugador.Nombre} compró {propiedad.Nombre} usando RFID.");
        }

        private Mensaje ProcesarNoComprar(Mensaje solicitud)
        {
            Jugador? jugador = _juego.ObtenerJugadorActual();
            return CrearRespuesta(solicitud, true, $"{jugador?.Nombre} decidió no comprar la propiedad.");
        }

        private Mensaje ProcesarPagar(Mensaje solicitud)
        {
            // Procesa la acción de pagar (activada mediante la lectura RFID o confirmación de cobro)
            string uid = "";
            if (solicitud.Datos.HasValue && solicitud.Datos.Value.TryGetProperty("uid", out var uidProp))
            {
                uid = uidProp.GetString() ?? "";
            }

            Jugador? jugador = _juego.ObtenerJugadorActual();
            if (jugador == null)
            {
                return CrearRespuesta(solicitud, false, "Jugador inválido.");
            }

            Console.WriteLine($"[Procesador] Pago confirmado vía RFID (UID: {uid}) para {jugador.Nombre}.");
            return CrearRespuesta(solicitud, true, $"Pago realizado exitosamente por {jugador.Nombre} (Tarjeta UID: {uid}).");
        }

        private Mensaje ProcesarTerminarTurno(Mensaje solicitud)
        {
            Jugador? anterior = _juego.ObtenerJugadorActual();
            _juego.TerminarTurno();
            Jugador? siguiente = _juego.ObtenerJugadorActual();

            if (_juego.JuegoTerminado())
            {
                Jugador? ganador = _juego.ObtenerGanador();
                string msgFinal = ganador != null ? $"Juego terminado. Ganador: {ganador.Nombre}" : "Juego terminado en empate.";
                return CrearRespuesta(solicitud, true, msgFinal);
            }

            return CrearRespuesta(solicitud, true, $"Turno de {anterior?.Nombre} finalizado. Ahora es el turno de {siguiente?.Nombre}.");
        }

        private Mensaje ProcesarConsultarEstado(Mensaje solicitud)
        {
            Jugador? actual = _juego.ObtenerJugadorActual();
            // Dueño de cada casilla: -1 = libre o no es propiedad. Se indexa por Id de casilla.
            // Se usa un arreglo simple (no List) y se recorre la lista circular del tablero.
            var cabeza = _juego.Tablero.Casillas.Cabeza;
            int totalCasillas = 0;
            if (cabeza != null)
            {
                var nodoConteo = cabeza;
                do { totalCasillas++; nodoConteo = nodoConteo.Siguiente; } while (nodoConteo != cabeza);
            }
            int[] duenos = new int[totalCasillas];
            for (int i = 0; i < totalCasillas; i++) duenos[i] = -1;
            if (cabeza != null && totalCasillas > 0)
            {
                var nodo = cabeza;
                do
                {
                    if (nodo.Dato is Propiedad prop && prop.Dueno != null)
                        duenos[nodo.Dato.Id % totalCasillas] = prop.Dueno.Id;
                    nodo = nodo.Siguiente;
                } while (nodo != cabeza);
            }

            // Los 4 jugadores en un arreglo, solo para poder recorrerlos al armar el estado
            var todos = new[] { _juego.Jugador1, _juego.Jugador2, _juego.Jugador3, _juego.Jugador4 };

            var estado = new
            {
                Turno = _juego.NumeroTurno,
                JugadorActualId = actual?.Id,
                JugadorActualNombre = actual?.Nombre,
                JugadoresActivos = _juego.CantidadJugadoresActivos(),
                JuegoTerminado = _juego.JuegoTerminado(),
                DadosLanzados = _juego.DadosLanzados,
                Duenos = duenos,
                // Solo hay ganador cuando la partida terminó (mayor patrimonio entre los activos)
                Ganador = _juego.JuegoTerminado() ? _juego.ObtenerGanador()?.Nombre : null,

                // Lo que la pantalla necesita para dibujar: el cliente solo lee esto,
                // nunca lo modifica (el estado oficial vive en el servidor)
                Jugadores = System.Linq.Enumerable.ToArray(
                    System.Linq.Enumerable.Select(todos, j => new
                    {
                        Id = j.Id,
                        Nombre = j.Nombre,
                        Saldo = j.Saldo,
                        Patrimonio = j.CalcularPatrimonio(), // saldo + valor de propiedades
                        Activo = j.Activo,
                        EnCarcel = j.EnCarcel,
                        // Posicion es un nodo de la lista circular: mandamos el Id de su casilla
                        Posicion = j.Posicion != null ? j.Posicion.Dato.Id : 0
                    }))
            };

            return new Mensaje
            {
                Accion = solicitud.Accion,
                IdJugador = solicitud.IdJugador,
                Exito = true,
                Descripcion = "Estado actual del juego.",
                Datos = JsonSerializer.SerializeToElement(estado)
            };
        }

        // Devuelve el historial de transacciones (de la más antigua a la más reciente)
        // y además regenera el archivo transacciones.txt en la carpeta del servidor.
        private Mensaje ProcesarConsultarTransacciones(Mensaje solicitud)
        {
            var banco = _juego.Banco;

            // Primero se cuenta cuántas hay, para usar un arreglo simple (sin List)
            int total = 0;
            var nodo = banco.Historial.Cabeza;
            while (nodo != null)
            {
                total++;
                nodo = nodo.Siguiente;
            }

            // Se recorre la lista doble desde la cabeza (más antigua) hasta la cola
            var items = new object[total];
            nodo = banco.Historial.Cabeza;
            int i = 0;
            while (nodo != null)
            {
                var t = nodo.Dato;
                items[i++] = new
                {
                    Id = t.Id,
                    Fecha = t.FechaHora.ToString(),
                    Turno = t.NumeroTurno,
                    Tipo = t.Tipo,
                    Origen = t.Origen?.Nombre ?? "Banco",
                    Destino = t.Destino?.Nombre ?? "Banco",
                    Monto = t.Monto,
                    Descripcion = t.Descripcion
                };
                nodo = nodo.Siguiente;
            }

            // Genera el TXT que pide el enunciado
            banco.ExportarHistorial("transacciones.txt");

            return new Mensaje
            {
                Accion = solicitud.Accion,
                IdJugador = solicitud.IdJugador,
                Exito = true,
                Descripcion = $"Historial: {total} transacciones. Archivo transacciones.txt generado.",
                Datos = JsonSerializer.SerializeToElement(items)
            };
        }

        private static Mensaje CrearRespuesta(Mensaje solicitud, bool exito, string descripcion)
        {
            return new Mensaje
            {
                Accion = solicitud.Accion,
                IdJugador = solicitud.IdJugador,
                Exito = exito,
                Descripcion = descripcion
            };
        }
    }
}