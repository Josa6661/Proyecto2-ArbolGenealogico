using Proyecto2_ArbolGenealogico.DataStructures;
using System;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestCola
    {
        public static void ProbarCola()
        {
            Console.WriteLine("=== INICIO TEST COLA ===");

            var cola = new Cola<int>();

            cola.Encolar(100);
            cola.Encolar(200);
            cola.Encolar(300);

            Console.WriteLine("Largo: " + cola.Largo()); 
            Console.WriteLine("Frente antes de desencolar: " + cola.VerFrente());

            Console.WriteLine("Desencolar: " + cola.Desencolar());
            Console.WriteLine("Frente tras desencolar: " + cola.VerFrente());

            Console.WriteLine("¿Está vacía?: " + cola.EstaVacia());

            cola.Limpiar();
            Console.WriteLine("¿Vacía tras limpiar?: " + cola.EstaVacia());

            Console.WriteLine("=== FIN TEST COLA ===");
        }
    }
}
