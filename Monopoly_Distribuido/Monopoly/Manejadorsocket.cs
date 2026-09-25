using System.Net.Sockets;
using System.Text;
using Monopoly.Protocolo;

namespace Monopoly.Comunicacion
{
    // Clase pequeña que envuelve un TcpClient para mandar y recibir mensajes completos.
    // Tanto el Servidor como el Cliente usan esta misma clase.
    // Los mensajes de red pueden llegar "cortados" o
    // "pegados". Entonces decidimos que cada mensaje se manda en una
    // sola línea de texto (JSON sin saltos de línea) terminada en '\n'.
    // Para que, del otro lado, se lea línea por línea.
    public class ManejadorSocket
    {
        private readonly NetworkStream _stream;
        private readonly StreamReader _lector;
        private readonly StreamWriter _escritor;

        public ManejadorSocket(TcpClient cliente)
        {
            _stream = cliente.GetStream();
            _lector = new StreamReader(_stream, Encoding.UTF8);

            _escritor = new StreamWriter(_stream, Encoding.UTF8);
            _escritor.AutoFlush = true; // manda el mensaje apenas se escribe, sin esperar buffer lleno
        }

        // Manda un Mensaje completo por el socket.
        public void Enviar(Mensaje mensaje)
        {
            string linea = mensaje.ToJson();
            _escritor.WriteLine(linea);
        }

        // Espera y devuelve el siguiente Mensaje que llegue por el socket.
        // Devuelve null si la conexión se cerró.
        public Mensaje? Recibir()
        {
            try
            {
                string? linea = _lector.ReadLine();
                if (linea == null)
                {
                    return null; // el otro lado cerró la conexión
                }
                return Mensaje.DesdeJson(linea);
            }
            catch (Exception)
            {
                return null; // Si la conexion falla abrubtamente
            }
        }

        public void Cerrar()
        {
            _lector.Close();
            _escritor.Close();
            _stream.Close();
        }
    }
}