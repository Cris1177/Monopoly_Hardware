using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Monopoly.Protocolo;

namespace Monopoly.Comunicacion
{
    // Esta interfaz se relaciona con la lógica del juego.
    // Quien programe Banco/Jugador/Tablero tiene que implementar esta interfaz
    // (por ejemplo en una clase Juego), y el Servidor solo se encarga de
    // recibir/mandar mensajes por la red, el servidor nada de reglas del juego.
    public interface IProcesadorAcciones
    {
        // Recibe la acción que pidió un jugador y devuelve el Mensaje de
        // respuesta (con Exito = true/false y una Descripcion).
        Mensaje Procesar(Mensaje solicitud);
    }

    public class Servidor
    {
        private readonly TcpListener _listener;
        private readonly IProcesadorAcciones _procesador;

        // Guarda el ManejadorSocket de cada jugador conectado, para poder
        // mandarle actualizaciones a todos cuando algo importante pasa
        // (ej. después de que alguien tira los dados).
        private readonly ConcurrentDictionary<int, ManejadorSocket> _clientesConectados = new();

        public Servidor(int puerto, IProcesadorAcciones procesador)
        {
            _listener = new TcpListener(IPAddress.Any, puerto);
            _procesador = procesador;
        }

        // Inicia el servidor: queda buscando conexiones nuevas para siempre.
        // Cada cliente que se conecta se atiende en su propio hilo, así el
        // servidor puede hablar con los 4 jugadores al mismo tiempo.
        public void Iniciar()
        {
            _listener.Start();
            Console.WriteLine("Servidor escuchando...");

            while (true)
            {
                TcpClient clienteTcp = _listener.AcceptTcpClient();
                Thread hilo = new Thread(() => AtenderCliente(clienteTcp));
                hilo.IsBackground = true;
                hilo.Start();
            }
        }

        private void AtenderCliente(TcpClient clienteTcp)
        {
            var manejador = new ManejadorSocket(clienteTcp);
            int idJugador = -1;

            try
            {
                while (true)
                {
                    Mensaje? solicitud = manejador.Recibir();
                    if (solicitud == null)
                    {
                        break; // el cliente cerró la conexión
                    }

                    // La primera vez que hablamos con este socket, guardamos
                    // su id para poder identificarlo en futuras difusiones.
                    if (solicitud.Accion == TipoAccion.CONECTAR)
                    {
                        idJugador = solicitud.IdJugador;
                        _clientesConectados[idJugador] = manejador;
                    }

                    // Aquí es donde se delega a la lógica del juego
                    // (Banco, Jugador, Tablero, etc.), ES UN STUB HASTA QUE SE AVANCE EL PROYECTO.
                    Mensaje respuesta = _procesador.Procesar(solicitud);

                    manejador.Enviar(respuesta);

                    // Después de una acción que cambia el estado del juego,
                    // hay que avisarle a TODOS los jugadores, no solo al que
                    // la pidió (por ejemplo, todos deben ver que alguien
                    // avanzó en el tablero).
                    if (respuesta.Exito == true && DebeDifundirse(solicitud.Accion))
                    {
                        DifundirATodos(respuesta);
                    }
                }
            }
            catch (IOException)
            {
                // La conexión se cayó de repente; lo tratamos igual que
                // una desconexión normal.
            }
            finally
            {
                if (idJugador != -1)
                {
                    _clientesConectados.TryRemove(idJugador, out _);
                }
                manejador.Cerrar();
            }
        }

        // Decide qué acciones deben notificarse a todos los jugadores y
        // cuáles son solo para quien las pidió (ej. CONSULTAR_ESTADO no
        // se necesita avisarle a todos).
        private bool DebeDifundirse(TipoAccion accion)
        {
            return accion is TipoAccion.TIRAR_DADOS
                or TipoAccion.COMPRAR_PROPIEDAD
                or TipoAccion.TERMINAR_TURNO
                or TipoAccion.PAGAR;
        }

        private void DifundirATodos(Mensaje mensaje)
        {
            foreach (var manejador in _clientesConectados.Values)
            {
                manejador.Enviar(mensaje);
            }
        }
    }
}