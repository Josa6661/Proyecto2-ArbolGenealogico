using Proyecto2_ArbolGenealogico.Controllers;
using Proyecto2_ArbolGenealogico.DataStructures;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestArbolController
    {
        public static void ProbarController()
        {
            Debug.WriteLine("=== INICIO TEST ARBOL CONTROLLER ===");

            var controller = new ArbolController();

            // Crear raíz
            controller.CrearRaiz("Abuelo", "1111", "1940-01-01", 85, "abuelo.jpg", 9.0, -84.0);

            // Agregar hijos
            controller.AgregarMiembro("Abuelo", "Padre", "2222", "1965-02-02", 60, "padre.jpg", 9.1, -84.1);
            controller.AgregarMiembro("Padre", "Hijo", "3333", "1990-03-03", 35, "hijo.jpg", 9.2, -84.2);

            // Buscar miembros
            var padre = controller.BuscarMiembro("Padre");
            var hijo = controller.BuscarMiembro("Hijo");
            var inexistente = controller.BuscarMiembro("Primo");

            Debug.WriteLine($"¿Padre encontrado?: {padre != null}");
            Debug.WriteLine($"¿Hijo encontrado?: {hijo != null}");
            Debug.WriteLine($"¿Primo encontrado?: {inexistente != null}");

            // Obtener todos los miembros
            var todos = controller.ObtenerTodos();
            Debug.WriteLine($"Cantidad total de miembros: {todos.Count}");

            // Eliminar un miembro
            controller.EliminarMiembro("Hijo");
            var despuesEliminar = controller.BuscarMiembro("Hijo");
            Debug.WriteLine($"¿Hijo sigue existiendo?: {despuesEliminar != null}");

            // Limpiar árbol
            controller.Limpiar();
            var despuesLimpiar = controller.BuscarMiembro("Abuelo");
            Debug.WriteLine($"¿Existe raíz tras limpiar?: {despuesLimpiar != null}");

            Debug.WriteLine("=== FIN TEST ARBOL CONTROLLER ===");
        }
    }
}
