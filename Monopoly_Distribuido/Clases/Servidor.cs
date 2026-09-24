using System.Net;
using System.Net.Sockets;

namespace Monopoly.Juego;

public class Servidor
{
    public Juego Juego { get; }
    public TcpListener Escuchador { get; }

    public Servidor(int puerto = 5000)
    {
        Juego = new Juego();
        Escuchador = new TcpListener(IPAddress.Any, puerto);
    }

    public void Iniciar()
    {
        Escuchador.Start();
    }

    public void Detener()
    {
        Escuchador.Stop();
    }
}
