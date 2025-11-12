using Proyecto2_ArbolGenealogico.DataStructures;
using System;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestArray
    {
        public static void ProbarArray()
        {
            Console.WriteLine("=== INICIO TEST ARRAY ===");

            var arr = new Array<int>();
            arr.Agregar(1);
            arr.Agregar(2);
            arr.Agregar(3);

            Console.WriteLine("Largo tras agregar: " + arr.Largo());
            Console.WriteLine("Elemento en 1: " + arr.Obtener(1));

            arr.Eliminar(1); // 
            Console.WriteLine("Largo tras eliminar posición 1: " + arr.Largo());

            arr.Limpiar();
            Console.WriteLine("Largo tras limpiar: " + arr.Largo());

            Console.WriteLine("=== FIN TEST ARRAY ===");
        }
    }
}
