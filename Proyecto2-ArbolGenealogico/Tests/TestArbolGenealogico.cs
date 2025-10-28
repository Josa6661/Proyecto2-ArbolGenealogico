using Proyecto2_ArbolGenealogico.DataStructures;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestArbolGenealogico
    {
        public static void ProbarArbol()
        {
            Debug.WriteLine("=== INICIO TEST ÁRBOL GENEALÓGICO ===");

            var arbol = new ArbolGenealogico();

            var abuelo = new NodoFamiliar("Abuelo", "1111", "1940-01-01", 85, "ruta.jpg", 9.0, -84.0);
            arbol.CrearRaiz(abuelo);

            var padre = new NodoFamiliar("Padre", "2222", "1965-02-02", 60, "rutaPadre.jpg", 9.1, -84.1);
            arbol.AgregarMiembro("Abuelo", padre);

            var hijo = new NodoFamiliar("Hijo", "4444", "1990-04-04", 35, "rutaHijo.jpg", 9.2, -84.2);
            arbol.AgregarMiembro("Padre", hijo);

            Debug.WriteLine("Buscar 'Padre': " + (arbol.BuscarPorNombre("Padre") != null));
            Debug.WriteLine("Buscar 'Hijo': " + (arbol.BuscarPorNombre("Hijo") != null));

            arbol.EliminarMiembro("Hijo");
            Debug.WriteLine("Buscar 'Hijo' tras eliminar: " + (arbol.BuscarPorNombre("Hijo") != null)); 

            arbol.Limpiar();
            Debug.WriteLine("Buscar 'Abuelo' tras limpiar: " + (arbol.BuscarPorNombre("Abuelo") != null));

            Debug.WriteLine("=== FIN TEST ÁRBOL GENEALÓGICO ===");
        }
    }
}