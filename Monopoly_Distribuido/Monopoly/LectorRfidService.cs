using System;
using Monopoly.Comunicacion;
using Monopoly.Juego;
using Monopoly.Protocolo;

namespace Monopoly
{
    public class LectorRfidService
    {
        private readonly Cliente? _clienteTcp;
        private readonly IProcesadorAcciones? _procesador;

        public LectorRfidService(IProcesadorAcciones procesador)
        {
            _procesador = procesador;
        }

        public LectorRfidService(Cliente clienteTcp)
        {
            _clienteTcp = clienteTcp;
        }

        public void Iniciar()
        {
            Console.WriteLine("[Lector RFID] Servicio iniciado. Esperando lecturas...");
        }

        public void Detener()
        {
            Console.WriteLine("[Lector RFID] Servicio detenido.");
        }

        public void ProcesarLecturaReal(string uid)
        {
            Console.WriteLine($"\n[RFID RECIBIDO] Tarjeta detectada con UID: ({uid})");

            if (_procesador != null)
            {
                // Cast a ProcesadorJuego para obtener el jugador activo mediante el nuevo método
                if (_procesador is ProcesadorJuego procesadorConcreto)
                {
                    var jugadorActual = procesadorConcreto.ObtenerJugadorActual();

                    if (jugadorActual == null)
                    {
                        Console.WriteLine("[ERROR RFID] No hay un jugador activo en este momento.");
                        return;
                    }

                    var mensajeAccion = new Mensaje
                    {
                        Accion = TipoAccion.COMPRAR_PROPIEDAD,
                        IdJugador = jugadorActual.Id
                    };

                    Console.WriteLine($"[RFID] Procesando compra para {jugadorActual.Nombre} (ID: {jugadorActual.Id})...");
                    var respuesta = _procesador.Procesar(mensajeAccion);

                    if (respuesta != null && respuesta.Exito == true)
                    {
                        Console.WriteLine($"[ÉXITO] ¡Propiedad comprada correctamente por {jugadorActual.Nombre}!");

                        //Cambio de turno despues de comprar
                        var mensajeFin = new Mensaje
                        {
                            Accion = TipoAccion.TERMINAR_TURNO,
                            IdJugador = jugadorActual.Id
                        };

                        var respFin = _procesador.Procesar(mensajeFin);
                        Console.WriteLine($"[TURNO] {respFin?.Descripcion}\n");
                    }
                    else
                    {
                        string detalle = respuesta?.Descripcion ?? "El banco rechazó la compra.";
                        Console.WriteLine($"[AVISO] {detalle}");
                    }
                }
                else
                {
                    var respuesta = _procesador.Procesar(new Mensaje { Accion = TipoAccion.COMPRAR_PROPIEDAD });
                    if (respuesta != null && respuesta.Exito == true)
                    {
                        Console.WriteLine("[ÉXITO] ¡Compra realizada correctamente!");
                    }
                    else
                    {
                        Console.WriteLine($"[AVISO] {respuesta?.Descripcion}");
                    }
                }
            }
        }
    }
}