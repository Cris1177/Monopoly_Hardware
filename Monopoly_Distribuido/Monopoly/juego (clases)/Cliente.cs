using System.Net.Sockets;

namespace Monopoly.Juego;

public class Cliente
{
    public TcpClient Conexion { get; }

    public Cliente()
    {
        Conexion = new TcpClient();
    }

    public Task ConectarAsync(string direccion, int puerto)
    {
        return Conexion.ConnectAsync(direccion, puerto);
    }
}
