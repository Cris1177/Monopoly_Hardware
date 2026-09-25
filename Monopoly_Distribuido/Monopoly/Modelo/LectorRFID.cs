using System;
using System.IO.Ports;
using System.Threading;

class Program
{
    private const string PuertoCom = "/dev/tty.usbserial-14610";
    private const int Baudios = 115200;

    static void Main(string[] args)
    {
        // Comando SAMConfiguration
        byte[] wakeup = new byte[] {
            0x55, 0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0xFF, 0x03, 0xFD, 0xD4, 0x14, 0x01, 0x17, 0x00
        };

        // Comando InListPassiveTarget (buscar tarjetas)
        byte[] buscarTarjeta = new byte[] {
            0x00, 0x00, 0xFF, 0x04, 0xFC, 0xD4, 0x4A, 0x01, 0x00, 0xE1, 0x00
        };

        using (SerialPort puerto = new SerialPort(PuertoCom, Baudios, Parity.None, 8, StopBits.One))
        {
            try
            {
                puerto.Open();
                Console.WriteLine($"Conectado al PN532 en {PuertoCom}");
                Console.WriteLine("Escaneando tarjetas NFC...\n");

                puerto.Write(wakeup, 0, wakeup.Length);
                Thread.Sleep(100);

                if (puerto.BytesToRead > 0)
                {
                    byte[] descarta = new byte[puerto.BytesToRead];
                    puerto.Read(descarta, 0, descarta.Length);
                }

                string ultimoUid = "";

                while (true)
                {
                    puerto.Write(buscarTarjeta, 0, buscarTarjeta.Length);
                    Thread.Sleep(150);

                    int bytes = puerto.BytesToRead;
                    if (bytes > 0)
                    {
                        byte[] respuesta = new byte[bytes];
                        puerto.Read(respuesta, 0, bytes);

                        // Si la respuesta incluye un UID válido de 4 bytes
                        if (respuesta.Length >= 20)
                        {
                            // Extracción del UID en la trama
                            string uid = $"{respuesta[19]:X2}:{respuesta[20]:X2}:{respuesta[21]:X2}:{respuesta[22]:X2}";

                            // Evitar imprimir repetidamente la misma tarjeta si se deja puesta
                            if (uid != ultimoUid)
                            {
                                Console.WriteLine($"🎯 Tarjeta / Llavero Detectado!");
                                Console.WriteLine($"   UID: {uid}\n");
                                ultimoUid = uid;
                            }
                        }
                    }
                    else
                    {
                        // Resetear para volver a detectar si retira la tarjeta
                        ultimoUid = "";
                    }

                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}