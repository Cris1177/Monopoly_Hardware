using System.Text.Json;

namespace Monopoly.Protocolo
{
    // Todas las acciones que un cliente le puede pedir al servidor. (Si el profe pide agregar una acción nueva, se agrega aquí.)
    public enum TipoAccion
    {
        CONECTAR,
        TIRAR_DADOS,
        COMPRAR_PROPIEDAD,
        NO_COMPRAR,
        TERMINAR_TURNO,
        CONSULTAR_ESTADO,
        CONSULTAR_TRANSACCIONES,
        PAGAR // usada cuando el jugador debe cubrir una compra/pago obligatorio
    }

    // Este es el "sobre" que viaja por el socket, tanto de cliente a servidor
    // como de servidor a cliente. Siempre se realiza un JSON antes de enviarse.
    public class Mensaje
    {
        // Qué acción se está pidiendo (o qué tipo de respuesta es).
        public TipoAccion Accion { get; set; }

        // A qué jugador pertenece este mensaje ( se usq para saber
        // quién está pidiendo qué, y para no dejar jugar fuera de turno).
        public int IdJugador { get; set; }

        // true/false cuando el servidor responde, indica si la acción se
        // pudo realizar.
        public bool? Exito { get; set; }

        // Mensaje legible para mostrarle al jugador (ej. "No tienes dinero suficiente").
        public string? Descripcion { get; set; }

        // Aquí va la información específica de cada acción, como JSON.
        // Por ejemplo, al comprar una propiedad, aquí va el id de la casilla.
        // Se deja como JsonElement para no tener que crear una clase distinta
        // por cada tipo de mensaje.
        public JsonElement? Datos { get; set; }

        // Serialización
        // Convierte este mensaje a una sola línea de texto JSON para mandarlo
        // por el socket. Usamos salto de línea como separador entre mensajes
        // (ver ManejadorSocket para el lado que lee).
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Mensaje? DesdeJson(string json)
        {
            return JsonSerializer.Deserialize<Mensaje>(json);
        }
    }
}