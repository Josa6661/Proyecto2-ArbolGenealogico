using Proyecto2_ArbolGenealogico.DataStructures;
using System;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestArbolGenealogico
    {
        public static void ProbarArbol()
        {
            Console.WriteLine("=== INICIO TEST �RBOL GENEAL�GICO ===");

            var arbol = new ArbolGenealogico();

            var abuelo = new NodoFamiliar("Abuelo", "1111", "1940-01-01", 85, "ruta.jpg", 9.0, -84.0);
            arbol.CrearRaiz(abuelo);

            var padre = new NodoFamiliar("Padre", "2222", "1965-02-02", 60, "rutaPadre.jpg", 9.1, -84.1);
            arbol.AgregarMiembro("Abuelo", padre);

            var hijo = new NodoFamiliar("Hijo", "4444", "1990-04-04", 35, "rutaHijo.jpg", 9.2, -84.2);
            arbol.AgregarMiembro("Padre", hijo);

            Console.WriteLine("Buscar 'Padre': " + (arbol.BuscarPorNombre("Padre") != null));
            Console.WriteLine("Buscar 'Hijo': " + (arbol.BuscarPorNombre("Hijo") != null));

            arbol.EliminarMiembro("Hijo");
            Console.WriteLine("Buscar 'Hijo' tras eliminar: " + (arbol.BuscarPorNombre("Hijo") != null)); 

            arbol.Limpiar();
            Console.WriteLine("Buscar 'Abuelo' tras limpiar: " + (arbol.BuscarPorNombre("Abuelo") != null));

            Console.WriteLine("=== FIN TEST �RBOL GENEAL�GICO ===");
        }
    }
}