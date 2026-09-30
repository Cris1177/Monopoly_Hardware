using Godot;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;


public partial class DadoSerialNode : Node
{
	
	[Signal]
	public delegate void DadoRecibidoEventHandler(int valor);

	[Export] public int Baudios = 115200;

	private SerialPort? _puerto;
	private Thread? _hilo;
	private volatile bool _activo; 

	public override void _Ready()
	{
		// Busca la Pico sola: en Mac aparece como /dev/cu.usbmodemXXXX
		string? nombre = SerialPort.GetPortNames()
			.FirstOrDefault(p => p.Contains("cu.usbmodem"));

		if (nombre == null)
		{
			GD.PrintErr("[Dado] No encontré la Pico. Revisa el cable USB.");
			return;
		}

		try
		{
			_puerto = new SerialPort(nombre, Baudios)
			{
				DtrEnable = true,
				RtsEnable = true,
				ReadTimeout = 500 
			};
			_puerto.Open();
			_puerto.DiscardInBuffer(); 
			GD.Print($"[Dado] Escuchando en {nombre}");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[Dado] No se pudo abrir {nombre}: {ex.Message}");
			return;
		}


		_activo = true;
		_hilo = new Thread(LeerLoop) { IsBackground = true };
		_hilo.Start();
	}

	private void LeerLoop()
	{
		while (_activo)
		{
			try
			{
				string linea = _puerto!.ReadLine().Trim();
				if (linea.StartsWith("DADO:") &&
					int.TryParse(linea.Substring(5).Trim(), out int valor))
				{
					GD.Print($"[Dado] Recibido: {valor}");
					
					Callable.From(() => EmitSignal(SignalName.DadoRecibido, valor)).CallDeferred();
				}
			}
			catch (TimeoutException) { } 
			catch (Exception ex)
			{
				GD.PrintErr($"[Dado] Error de lectura: {ex.Message}");
				Thread.Sleep(200);
			}
		}
	}

	public override void _ExitTree()
	{
		// Libera el puerto al cerrar el juego, si no queda ocupado
		_activo = false;
		_hilo?.Join(1000);
		if (_puerto != null && _puerto.IsOpen) _puerto.Close();
	}
}