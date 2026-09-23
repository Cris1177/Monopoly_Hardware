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
        tablero.Add(new CasillaMonopoly("Avenida Mediterráneo", "Propiedad", 60));
        tablero.Add(new CasillaMonopoly("Caja de Comunidad", "Comunidad", 0));
        tablero.Add(new CasillaMonopoly("Avenida Báltico", "Propiedad", 60));
        tablero.Add(new CasillaMonopoly("Impuesto sobre la Renta", "Impuesto", 200));
        tablero.Add(new CasillaMonopoly("Ferrocarril de Reading", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Avenida Oriental", "Propiedad", 100));
        tablero.Add(new CasillaMonopoly("Suerte", "Suerte", 0));
        tablero.Add(new CasillaMonopoly("Avenida Vermont", "Propiedad", 100));
        tablero.Add(new CasillaMonopoly("Avenida Connecticut", "Propiedad", 120));
        tablero.Add(new CasillaMonopoly("Cárcel / Solo de Visita", "Carcel", 0));
        tablero.Add(new CasillaMonopoly("Plaza St. Charles", "Propiedad", 140));
        tablero.Add(new CasillaMonopoly("Compañía Eléctrica", "Servicio", 150));
        tablero.Add(new CasillaMonopoly("Avenida States", "Propiedad", 140));
        tablero.Add(new CasillaMonopoly("Avenida Virginia", "Propiedad", 160));
        tablero.Add(new CasillaMonopoly("Ferrocarril de Pennsylvania", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Plaza Guapiles de Limon", "Propiedad", 180));
        tablero.Add(new CasillaMonopoly("Caja de Comunidad", "Comunidad", 0));
        tablero.Add(new CasillaMonopoly("Avenida Alajuelita", "Propiedad", 180));
        tablero.Add(new CasillaMonopoly("Avenida New York", "Propiedad", 200));
        tablero.Add(new CasillaMonopoly("Parqueo Gratis", "ParqueoGratis", 0));
        tablero.Add(new CasillaMonopoly("Avenida Cartago City", "Propiedad", 220));
        tablero.Add(new CasillaMonopoly("Suerte", "Suerte", 0));
        tablero.Add(new CasillaMonopoly("Avenida Desamparados", "Propiedad", 220));
        tablero.Add(new CasillaMonopoly("Avenida Illinois", "Propiedad", 240));
        tablero.Add(new CasillaMonopoly("Ferrocarril B&O", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Avenida Atlantic", "Propiedad", 260));
        tablero.Add(new CasillaMonopoly("Avenida Ventnor", "Propiedad", 260));
        tablero.Add(new CasillaMonopoly("Compañía de Agua", "Servicio", 150));
        tablero.Add(new CasillaMonopoly("Jardines Marvin", "Propiedad", 280));
        tablero.Add(new CasillaMonopoly("Ir a la Cárcel", "IrCarcel", 0));
        tablero.Add(new CasillaMonopoly("Avenida Pacific", "Propiedad", 300));
        tablero.Add(new CasillaMonopoly("Avenida Jaco", "Propiedad", 300));
        tablero.Add(new CasillaMonopoly("Caja de Comunidad", "Comunidad", 0));
        tablero.Add(new CasillaMonopoly("Avenida Pennsylvania", "Propiedad", 320));
        tablero.Add(new CasillaMonopoly("Ferrocarril Short Line", "Ferrocarril", 200));
        tablero.Add(new CasillaMonopoly("Suerte", "Suerte", 0));
        tablero.Add(new CasillaMonopoly("Plaza Park", "Propiedad", 350));
        tablero.Add(new CasillaMonopoly("Impuesto de Lujo", "Impuesto", 100));
        tablero.Add(new CasillaMonopoly("Paseo Tablado", "Propiedad", 400));

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
