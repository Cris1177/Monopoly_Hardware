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
    // 0
    Casillas.Agregar(
        new CasillaEspecial(
            0,
            "Salida",
            TipoCasillaEspecial.Inicio
        )
    );

    // 1
    Casillas.Agregar(
        new Propiedad(
            1,
            "Avenida Coto Brus",
            60m,
            10m
        )
    );

    // 2
    Casillas.Agregar(
        new CasillaEvento(
            2,
            "CCSS"
        )
    );

    // 3
    Casillas.Agregar(
        new Propiedad(
            3,
            "Avenida Belén Heredia",
            60m,
            10m
        )
    );

    // 4
    Casillas.Agregar(
        new CasillaEspecial(
            4,
            "Impuesto sobre la Renta",
            TipoCasillaEspecial.Impuesto,
            200m
        )
    );

    // 5
    Casillas.Agregar(
        new Propiedad(
            5,
            "Ferrocarril de Reading",
            200m,
            25m
        )
    );

    // 6
    Casillas.Agregar(
        new Propiedad(
            6,
            "Avenida Agua Caliente",
            100m,
            15m
        )
    );

    // 7
    Casillas.Agregar(
        new CasillaEvento(
            7,
            "Suerte"
        )
    );

    // 8
    Casillas.Agregar(
        new Propiedad(
            8,
            "Avenida Vermont",
            100m,
            15m
        )
    );

    // 9
    Casillas.Agregar(
        new Propiedad(
            9,
            "Avenida Manuel de Jesús",
            120m,
            20m
        )
    );

    // 10
    Casillas.Agregar(
        new CasillaEspecial(
            10,
            "Cárcel / Solo de Visita",
            TipoCasillaEspecial.Carcel
        )
    );

    // 11
    Casillas.Agregar(
        new Propiedad(
            11,
            "Plaza St. Charles",
            140m,
            25m
        )
    );

    // 12
    Casillas.Agregar(
        new Propiedad(
            12,
            "Compañía ICE",
            150m,
            30m
        )
    );

    // 13
    Casillas.Agregar(
        new Propiedad(
            13,
            "Avenida Aurora",
            140m,
            25m
        )
    );

    // 14
    Casillas.Agregar(
        new Propiedad(
            14,
            "Avenida Purral",
            160m,
            30m
        )
    );

    // 15
    Casillas.Agregar(
        new Propiedad(
            15,
            "Ferrocarril de Pacífico",
            200m,
            35m
        )
    );

    // 16
    Casillas.Agregar(
        new Propiedad(
            16,
            "Plaza Guápiles de Limón",
            180m,
            35m
        )
    );

    // 17
    Casillas.Agregar(
        new CasillaEvento(
            17,
            "CCSS"
        )
    );

    // 18
    Casillas.Agregar(
        new Propiedad(
            18,
            "Avenida Alajuelita",
            180m,
            35m
        )
    );

    // 19
    Casillas.Agregar(
        new Propiedad(
            19,
            "Avenida San Pedro",
            200m,
            40m
        )
    );

    // 20
    Casillas.Agregar(
        new CasillaEspecial(
            20,
            "Parque del TEC",
            TipoCasillaEspecial.ParqueoGratis
        )
    );

    // 21
    Casillas.Agregar(
        new Propiedad(
            21,
            "Avenida Cartago City",
            220m,
            45m
        )
    );

    // 22
    Casillas.Agregar(
        new CasillaEvento(
            22,
            "Suerte"
        )
    );

    // 23
    Casillas.Agregar(
        new Propiedad(
            23,
            "Avenida Desamparados",
            220m,
            45m
        )
    );
    
        }
    }
}