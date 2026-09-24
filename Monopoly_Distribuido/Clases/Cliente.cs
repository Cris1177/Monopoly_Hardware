using System.Net.Sockets;
using System.Text.Json;
using Monopoly.Protocolo;

namespace Monopoly.Comunicacion
{
    public class Cliente
    {
        private readonly TcpClient _tcp;
        private readonly ManejadorSocket _manejador;
        public int IdJugador { get; }

        // Evento al que se suscribe la interfaz gráfica (Godot) para
        // enterarse cuando llega una actualización del servidor sin tener
        // que estar preguntando todo el tiempo.
        public event Action<Mensaje>? MensajeRecibido;

        public Cliente(string ip, int puerto, int idJugador)
        {
            IdJugador = idJugador;
            _tcp = new TcpClient(ip, puerto);
            _manejador = new ManejadorSocket(_tcp);
        }

        // Se conecta y avisa al servidor quién es el jugador.
        public void Conectar()
        {
            EnviarAccion(TipoAccion.CONECTAR);

            // Arranca un hilo aparte que solo escucha mensajes que lleguen
            // del servidor (respuestas y difusiones), para no bloquear
            // el resto del programa mientras se espera.
            Thread hiloEscucha = new Thread(EscucharServidor);
            hiloEscucha.IsBackground = true;
            hiloEscucha.Start();
        }

        // Manda una acción al servidor. 'datos' es opcional: cualquier
        // objeto que se necesite mandar (ej. el id de la propiedad a comprar).
        public void EnviarAccion(TipoAccion accion, object? datos = null)
        {
            var mensaje = new Mensaje
            {
                Accion = accion,
                IdJugador = IdJugador,
                Datos = datos != null
                    ? JsonSerializer.SerializeToElement(datos)
                    : null
            };
            _manejador.Enviar(mensaje);
        }

        private void EscucharServidor()
        {
            while (true)
            {
                Mensaje? mensaje = _manejador.Recibir();
                if (mensaje == null)
                {
                    break; // el servidor cerró la conexión
                }
                // Notifica a quien esté escuchando (ej. la UI de Godot)
                MensajeRecibido?.Invoke(mensaje);
            }
        }

        public void Desconectar()
        {
            _manejador.Cerrar();
            _tcp.Close();
        }
    }
}