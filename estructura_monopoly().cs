using System;
using System.Collections.Generic;

class MonopolyProgramEstructura
{
    static void Main()
    {
        // Crear tablero
        List<CasillaMonopoly> tablero = CrearTablero();

        // Crear jugadores
        List<JugadorMonopoly> jugadores = new List<JugadorMonopoly>();

        jugadores.Add(new JugadorMonopoly("Cristian"));
        jugadores.Add(new JugadorMonopoly("Isaac"));
        jugadores.Add(new JugadorMonopoly("Ulfrán"));
        jugadores.Add(new JugadorMonopoly("Fabricio"));

        // Mostrar jugadores
        Console.WriteLine("JUGADORES");

        for (int i = 0; i < jugadores.Count; i++)
        {
            Console.WriteLine(
                jugadores[i].Nombre +
                " - Dinero: $" +
                jugadores[i].Dinero +
                " - Posición: " +
                jugadores[i].Posicion
            );
        }
    }

    static List<CasillaMonopoly> CrearTablero()
    {
        List<CasillaMonopoly> tablero = new List<CasillaMonopoly>();

                tablero.Add(new CasillaMonopoly("Salida", "Salida", 0));
        tablero.Add(new CasillaMonopoly("Avenida Coto Brus", "Propiedad", 60));
        tablero.Add(new CasillaMonopoly("CCSS", "Comunidad", 0));
        tablero.Add(new CasillaMonopoly("Avenida Belen Heredia", "Propiedad", 60));
        tablero.Add(new CasillaMonopoly("Impuesto sobre la Renta", "Impuesto", 200));
        tablero.Add(new CasillaMonopoly("Ferrocarril de Reading", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Avenida Agua Caliente", "Propiedad", 100));
        tablero.Add(new CasillaMonopoly("Suerte", "Suerte", 0));
        tablero.Add(new CasillaMonopoly("Avenida Vermont", "Propiedad", 100));
        tablero.Add(new CasillaMonopoly("Avenida Manuel de Jesus", "Propiedad", 120));
        tablero.Add(new CasillaMonopoly("Cárcel / Solo de Visita", "Carcel", 0));
        tablero.Add(new CasillaMonopoly("Plaza St. Charles", "Propiedad", 140));
        tablero.Add(new CasillaMonopoly("Compañía ICE", "Servicio", 150));
        tablero.Add(new CasillaMonopoly("Avenida Aurora", "Propiedad", 140));
        tablero.Add(new CasillaMonopoly("Avenida Purral", "Propiedad", 160));
        tablero.Add(new CasillaMonopoly("Ferrocarril de Pacifico", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Plaza Guapiles de Limon", "Propiedad", 180));
        tablero.Add(new CasillaMonopoly("CCSS", "Comunidad", 0));
        tablero.Add(new CasillaMonopoly("Avenida Alajuelita", "Propiedad", 180));
        tablero.Add(new CasillaMonopoly("Avenida San Pedro", "Propiedad", 200));
        tablero.Add(new CasillaMonopoly("Parque del TEC", "ParqueoGratis", 0));
        tablero.Add(new CasillaMonopoly("Avenida Cartago City", "Propiedad", 220));
        tablero.Add(new CasillaMonopoly("Suerte", "Suerte", 0));
        tablero.Add(new CasillaMonopoly("Avenida Desamparados", "Propiedad", 220));
        tablero.Add(new CasillaMonopoly("Avenida Grecia", "Propiedad", 240));
        tablero.Add(new CasillaMonopoly("Ferrocarril del Atlantico", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Avenida Nicoya Centro", "Propiedad", 260));
        tablero.Add(new CasillaMonopoly("Avenida Tobosi", "Propiedad", 260));
        tablero.Add(new CasillaMonopoly("Compañía ICE", "Servicio", 150));
        tablero.Add(new CasillaMonopoly("Jardines Marvin", "Propiedad", 280));
        tablero.Add(new CasillaMonopoly("Ir a la TABO", "IrCarcel", 0));
        tablero.Add(new CasillaMonopoly("Calle de la Amargaura", "Propiedad", 300));
        tablero.Add(new CasillaMonopoly("Avenida Jaco", "Propiedad", 300));
        tablero.Add(new CasillaMonopoly("Caja de Comunidad", "Comunidad", 0));
        tablero.Add(new CasillaMonopoly("Avenida Fuente de la Hispanidad", "Propiedad", 320));
        tablero.Add(new CasillaMonopoly("Ferrocarril De Costa Rica", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Suerte", "Suerte", 0));
        tablero.Add(new CasillaMonopoly("Parque la Francia", "Propiedad", 350));
        tablero.Add(new CasillaMonopoly("Impuesto de Lujo", "Impuesto", 100));
        tablero.Add(new CasillaMonopoly("Paseo Metropoli", "Propiedad", 400));

        return tablero;
    }
}

class JugadorMonopoly
{
    public string Nombre;
    public int Dinero;
    public int Posicion;

    public JugadorMonopoly(string nombre)
    {
        Nombre = nombre;
        Dinero = 1500;
        Posicion = 0;
    }
}

class CasillaMonopoly
{
    public string Nombre;
    public string Tipo;
    public int Precio;

    public CasillaMonopoly(string nombre, string tipo, int precio)
    {
        Nombre = nombre;
        Tipo = tipo;
        Precio = precio;
    }
}
