using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Monopoly.Protocolo;

namespace Monopoly.Comunicacion
{
    // Esta interfaz es el "gancho" hacia la lógica real del juego.
    // El compañero que hace Banco/Jugador/Tablero implementa esta interfaz
    // (por ejemplo en una clase Juego), y el Servidor solo se encarga de
    // recibir/mandar mensajes por la red, sin saber nada de reglas del juego.
    //
    // Así, esta parte (cliente-servidor) se puede probar y avanzar ya,
    // aunque las clases de juego todavía sean stubs.
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

        // Arranca el servidor: queda escuchando conexiones nuevas para siempre.
        // Cada cliente que se conecta se atiende en su propio hilo, así el
        // servidor puede hablar con los 4 jugadores al mismo tiempo.
        public void Iniciar()
        {
            _listener.Start();
            Console.WriteLine("Servidor escuchando...");
            Console.WriteLine("(Presiona Ctrl+C para detener el servidor antes de cerrar la terminal)");

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

                    // Aquí es donde se delega a la lógica real del juego
                    // (Banco, Jugador, Tablero, etc.), que todavía puede
                    // ser un stub mientras el resto del equipo avanza.
                    Mensaje respuesta = _procesador.Procesar(solicitud);

                    manejador.Enviar(respuesta);

                    // Después de una acción que cambia el estado del juego,
                    // hay que avisarle a los DEMÁS jugadores (no a quien ya
                    // recibió la respuesta directa arriba, para no mandarle
                    // el mismo mensaje dos veces) que algo pasó, por ejemplo
                    // que alguien avanzó en el tablero.
                    if (respuesta.Exito == true && DebeDifundirse(solicitud.Accion))
                    {
                        DifundirATodosMenos(respuesta, idJugador);
                    }
                }
            }
            catch (IOException)
            {
                // La conexión se cayó abruptamente; lo tratamos igual que
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
        // necesita avisarle a nadie más).
        private bool DebeDifundirse(TipoAccion accion)
        {
            return accion is TipoAccion.TIRAR_DADOS
                or TipoAccion.COMPRAR_PROPIEDAD
                or TipoAccion.TERMINAR_TURNO
                or TipoAccion.PAGAR;
        }

        // Manda el mensaje a todos los jugadores conectados EXCEPTO al que
        // se pasa en idAExcluir (normalmente quien ya recibió la respuesta
        // directa, para que no le llegue el mismo mensaje dos veces).
        private void DifundirATodosMenos(Mensaje mensaje, int idAExcluir)
        {
            foreach (var (idJugador, manejador) in _clientesConectados)
            {
                if (idJugador == idAExcluir)
                {
                    continue;
                }
                manejador.Enviar(mensaje);
            }
        }
    }
}