using Proyecto2_ArbolGenealogico.DataStructures;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestCola
    {
        public static void ProbarCola()
        {
            Debug.WriteLine("=== INICIO TEST COLA ===");

            var cola = new Cola<int>();

            cola.Encolar(100);
            cola.Encolar(200);
            cola.Encolar(300);

            Debug.WriteLine("Largo: " + cola.Largo()); 
            Debug.WriteLine("Frente antes de desencolar: " + cola.VerFrente());

            Debug.WriteLine("Desencolar: " + cola.Desencolar());
            Debug.WriteLine("Frente tras desencolar: " + cola.VerFrente());

            Debug.WriteLine("¿Está vacía?: " + cola.EstaVacia());

            cola.Limpiar();
            Debug.WriteLine("¿Vacía tras limpiar?: " + cola.EstaVacia());

            Debug.WriteLine("=== FIN TEST COLA ===");
        }
    }
}
