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

        // Constructor para modo Hardware local
        public LectorRfidService(IProcesadorAcciones procesador)
        {
            _procesador = procesador;
        }

        // Constructor para modo Red / Cliente TCP
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
            Console.WriteLine($"\n[RFID RECIBIDO] Tarjeta detectada: ({uid})");

            if (_procesador == null)
            {
                Console.WriteLine("[ERROR RFID] El procesador de acciones está NULL en LectorRfidService.");
                return;
            }

            // 1. Obtener al jugador en turno desde el procesador
            var jugadorActual = _procesador.ObtenerJugadorActual();

            if (jugadorActual == null)
            {
                Console.WriteLine("[ERROR RFID] No hay un jugador activo en este momento.");
                return;
            }

            // 2. Crear el mensaje asignando el ID del jugador en turno
            var mensajeAccion = new Mensaje
            {
                Accion = TipoAccion.COMPRAR_PROPIEDAD,
                IdJugador = jugadorActual.Id // Asignamos el ID del jugador activo (Christian, etc.)
            };

            Console.WriteLine($"[RFID] Intentando ejecutar COMPRAR_PROPIEDAD para {jugadorActual.Nombre} (ID: {jugadorActual.Id})...");
            var respuesta = _procesador.Procesar(mensajeAccion);

            if (respuesta != null && respuesta.Exito == true)
            {
                Console.WriteLine($"[ÉXITO] ¡Compra realizada correctamente por {jugadorActual.Nombre}!");
            }
            else
            {
                string detalle = respuesta?.Descripcion ?? "El motor de juego rechazó la compra.";
                Console.WriteLine($"[AVISO] {detalle}");
            }
        }
    }
}