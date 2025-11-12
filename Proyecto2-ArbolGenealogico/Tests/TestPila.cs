using Proyecto2_ArbolGenealogico.DataStructures;
using System;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestPila
    {
        public static void ProbarPila()
        {
            Console.WriteLine("=== INICIO TEST PILA ===");

            var pila = new Pila<int>();

            pila.Apilar(10);
            pila.Apilar(20);
            pila.Apilar(30);

            Console.WriteLine("Largo: " + pila.Largo());
            Console.WriteLine("Tope antes de desapilar: " + pila.VerTope()); 

            Console.WriteLine("Desapilar: " + pila.Desapilar());
            Console.WriteLine("Tope tras desapilar: " + pila.VerTope());

            Console.WriteLine("¿Está vacía?: " + pila.EstaVacia());

            pila.Limpiar();
            Console.WriteLine("¿Vacía tras limpiar?: " + pila.EstaVacia());

            Console.WriteLine("=== FIN TEST PILA ===");
        }
    }
}
