using Proyecto2_ArbolGenealogico.Services;
using Proyecto2_ArbolGenealogico.DataStructures;
using System;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestArbolService
    {
        public static void ProbarService()
        {
            Console.WriteLine("=== INICIO TEST ARBOL SERVICE ===");

            var service = ArbolService.Instancia;

            // Asegurar que el árbol está limpio antes de comenzar
            service.Limpiar();

            // Crear raíz
            var abuelo = new NodoFamiliar("Abuelo", "1111", "1940-01-01", 85, "abuelo.jpg", 9.0, -84.0);
            bool raizCreada = service.CrearRaiz(abuelo);
            Console.WriteLine("Raíz creada correctamente: " + raizCreada);

            // Agregar miembros
            var padre = new NodoFamiliar("Padre", "2222", "1965-02-02", 60, "padre.jpg", 9.1, -84.1);
            var hijo = new NodoFamiliar("Hijo", "3333", "1990-03-03", 35, "hijo.jpg", 9.2, -84.2);

            bool agregadoPadre = service.AgregarMiembro("Abuelo", padre);
            bool agregadoHijo = service.AgregarMiembro("Padre", hijo);

            Console.WriteLine("Agregado Padre: " + agregadoPadre);
            Console.WriteLine("Agregado Hijo: " + agregadoHijo);

            // Buscar miembros
            Console.WriteLine("Buscar 'Abuelo': " + (service.BuscarMiembro("Abuelo") != null));
            Console.WriteLine("Buscar 'Padre': " + (service.BuscarMiembro("Padre") != null));
            Console.WriteLine("Buscar 'Hijo': " + (service.BuscarMiembro("Hijo") != null));

            // Listar todos los miembros
            var todos = service.ObtenerTodos();
            Console.WriteLine("Cantidad total de miembros: " + todos.Count);

            // Eliminar un miembro
            bool eliminado = service.EliminarMiembro("Hijo");
            Console.WriteLine("Eliminar 'Hijo': " + eliminado);
            Console.WriteLine("Buscar 'Hijo' tras eliminar: " + (service.BuscarMiembro("Hijo") != null));

            // Limpiar árbol
            service.Limpiar();
            Console.WriteLine("Buscar 'Abuelo' tras limpiar: " + (service.BuscarMiembro("Abuelo") != null));

            Console.WriteLine("=== FIN TEST ARBOL SERVICE ===");
        }
    }
}