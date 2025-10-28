using Proyecto2_ArbolGenealogico.DataStructures;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestPila
    {
        public static void ProbarPila()
        {
            Debug.WriteLine("=== INICIO TEST PILA ===");

            var pila = new Pila<int>();

            pila.Apilar(10);
            pila.Apilar(20);
            pila.Apilar(30);

            Debug.WriteLine("Largo: " + pila.Largo());
            Debug.WriteLine("Tope antes de desapilar: " + pila.VerTope()); 

            Debug.WriteLine("Desapilar: " + pila.Desapilar());
            Debug.WriteLine("Tope tras desapilar: " + pila.VerTope());

            Debug.WriteLine("¿Está vacía?: " + pila.EstaVacia());

            pila.Limpiar();
            Debug.WriteLine("¿Vacía tras limpiar?: " + pila.EstaVacia());

            Debug.WriteLine("=== FIN TEST PILA ===");
        }
    }
}
