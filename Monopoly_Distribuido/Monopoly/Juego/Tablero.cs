using Monopoly.Estructuras;
using Monopoly.Modelos;

namespace Monopoly.Juego
{
    public class Tablero
    {
        public ListaDobleCircular Casillas { get; private set; }

        public Tablero()
        {
            Casillas = new ListaDobleCircular();
            CrearCasillas();
        }

        private void CrearCasillas()
        {
            // 0 - Salida
            Casillas.Agregar(new CasillaEspecial(0, "Salida", TipoCasillaEspecial.Inicio));

            // 1 - 9
            Casillas.Agregar(new Propiedad(1, "Avenida Coto Brus", 60m, 10m));
            Casillas.Agregar(new CasillaEvento(2, "CCSS"));
            Casillas.Agregar(new Propiedad(3, "Avenida Belén Heredia", 60m, 10m));
            Casillas.Agregar(new CasillaEspecial(4, "Impuesto sobre la Renta", TipoCasillaEspecial.Impuesto, 200m));
            Casillas.Agregar(new Propiedad(5, "Ferrocarril de Reading", 200m, 25m));
            Casillas.Agregar(new Propiedad(6, "Avenida Agua Caliente", 100m, 15m));
            Casillas.Agregar(new CasillaEvento(7, "Suerte"));
            Casillas.Agregar(new Propiedad(8, "Avenida Vermont", 100m, 15m));
            Casillas.Agregar(new Propiedad(9, "Avenida Lyndon B.Jonhson", 120m, 20m));

            // 10 - Cárcel
            Casillas.Agregar(new CasillaEspecial(10, "Cárcel / Solo de Visita", TipoCasillaEspecial.Carcel));

            // 11 - 19
            Casillas.Agregar(new Propiedad(11, "Plaza St. Charles", 140m, 25m));
            Casillas.Agregar(new Propiedad(12, "Compañía ICE", 150m, 30m));
            Casillas.Agregar(new Propiedad(13, "Avenida Aurora", 140m, 25m));
            Casillas.Agregar(new Propiedad(14, "Avenida Purral", 160m, 30m));
            Casillas.Agregar(new Propiedad(15, "Ferrocarril de Pacífico", 200m, 35m));
            Casillas.Agregar(new Propiedad(16, "Plaza Guápiles de Limón", 180m, 35m));
            Casillas.Agregar(new CasillaEvento(17, "Compañia Telecable"));
            Casillas.Agregar(new Propiedad(18, "Avenida Alajuelita", 180m, 35m));
            Casillas.Agregar(new Propiedad(19, "Avenida San Pedro", 200m, 40m));

            // 20 - Parqueo Gratis
            Casillas.Agregar(new CasillaEspecial(20, "Parque del TEC", TipoCasillaEspecial.ParqueoGratis));

            // 21 - 29
            Casillas.Agregar(new Propiedad(21, "Avenida Cartago City", 220m, 45m));
            Casillas.Agregar(new CasillaEvento(22, "Suerte"));
            Casillas.Agregar(new Propiedad(23, "Avenida Desamparados", 220m, 45m));
            Casillas.Agregar(new Propiedad(24, "Avenida Grecia", 240m, 50m));
            Casillas.Agregar(new Propiedad(25, "Ferrocarril del Atlántico", 200m, 35m));
            Casillas.Agregar(new Propiedad(26, "Avenida Nicoya Centro", 260m, 55m));
            Casillas.Agregar(new Propiedad(27, "Avenida Tobosi", 260m, 55m));
            Casillas.Agregar(new Propiedad(28, "Compañía JASEC", 150m, 30m));
            Casillas.Agregar(new Propiedad(29, "Jardines Marvin", 280m, 60m));

            // 30 - Ir a la Cárcel
            Casillas.Agregar(new CasillaEspecial(30, "Ir a la TABO", TipoCasillaEspecial.IrCarcel));

            // 31 - 39
            Casillas.Agregar(new Propiedad(31, "Calle de la Amargura", 300m, 65m));
            Casillas.Agregar(new Propiedad(32, "Avenida Jacó", 300m, 65m));
            Casillas.Agregar(new CasillaEvento(33, "Caja de Comunidad"));
            Casillas.Agregar(new Propiedad(34, "Avenida Fuente de la Hispanidad", 320m, 70m));
            Casillas.Agregar(new Propiedad(35, "Ferrocarril De Costa Rica", 200m, 35m));
            Casillas.Agregar(new CasillaEvento(36, "Suerte"));
            Casillas.Agregar(new Propiedad(37, "Parque la Francia", 350m, 75m));
            Casillas.Agregar(new CasillaEspecial(38, "Impuesto de Lujo", TipoCasillaEspecial.Impuesto, 100m));
            Casillas.Agregar(new Propiedad(39, "Paseo Metrópoli", 400m, 80m));
        }
    }
}