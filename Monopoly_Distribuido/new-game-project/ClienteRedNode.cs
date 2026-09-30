using Godot;
using System;
using Monopoly.Comunicacion;
using Monopoly.Protocolo;

// Nodo que conecta la pantalla (GDScript) con el servidor por TCP.
// La pantalla NUNCA decide nada: pide acciones y dibuja el estado que manda el servidor.
public partial class ClienteRedNode : Node
{
	// Llega el estado completo (JSON) cuando el servidor responde CONSULTAR_ESTADO
	[Signal]
	public delegate void EstadoRecibidoEventHandler(string json);

	// Cualquier otra respuesta o difusión del servidor (para mostrar en el log)
	[Signal]
	public delegate void MensajeServidorEventHandler(string accion, bool exito, string descripcion);

	private Cliente? _cliente;
	private bool _conectado;
	private double _tiempo; // acumula segundos para pedir el estado cada 1 s

	// Devuelve true si logró conectarse. Lo llama GameBoard.gd al iniciar.
	public bool Conectar(string ip, int idJugador)
	{
		try
		{
			_cliente = new Cliente(ip, 5000, idJugador);
			// Este evento se dispara en el hilo de red: no tocar la interfaz aquí directamente
			_cliente.MensajeRecibido += AlRecibir;
			_cliente.Conectar(); // manda CONECTAR y arranca el hilo que escucha
			_conectado = true;
			GD.Print($"[Red] Conectado a {ip}:5000 como jugador {idJugador}");
			return true;
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[Red] No se pudo conectar a {ip}: {ex.Message}");
			return false;
		}
	}

	// GameBoard.gd llama esto con el nombre de la acción: "TIRAR_DADOS", "TERMINAR_TURNO"...
	public void Enviar(string accion)
	{
		if (!_conectado || _cliente == null) return;
		if (Enum.TryParse<TipoAccion>(accion, out var a))
			_cliente.EnviarAccion(a);
		else
			GD.PrintErr($"[Red] Acción desconocida: {accion}");
	}

	// Pide el estado al servidor una vez por segundo
	public override void _Process(double delta)
	{
		if (!_conectado || _cliente == null) return;
		_tiempo += delta;
		if (_tiempo >= 1.0)
		{
			_tiempo = 0;
			_cliente.EnviarAccion(TipoAccion.CONSULTAR_ESTADO);
		}
	}

	// Corre en el hilo de red: pasa todo al hilo principal con CallDeferred
	private void AlRecibir(Mensaje msg)
	{
		if (msg.Accion == TipoAccion.CONSULTAR_ESTADO && msg.Datos.HasValue)
		{
			string json = msg.Datos.Value.GetRawText();
			Callable.From(() => EmitSignal(SignalName.EstadoRecibido, json)).CallDeferred();
		}
		else
		{
			string accion = msg.Accion.ToString();
			bool exito = msg.Exito ?? false;
			string desc = msg.Descripcion ?? "";
			Callable.From(() => EmitSignal(SignalName.MensajeServidor, accion, exito, desc)).CallDeferred();
		}
	}

	public override void _ExitTree()
	{
		_conectado = false;
		try { _cliente?.Desconectar(); } catch { }
	}
}
