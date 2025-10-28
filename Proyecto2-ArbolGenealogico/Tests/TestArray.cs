using Proyecto2_ArbolGenealogico.DataStructures;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestArray
    {
        public static void ProbarArray()
        {
            Debug.WriteLine("=== INICIO TEST ARRAY ===");

            var arr = new Array<int>();
            arr.Agregar(1);
            arr.Agregar(2);
            arr.Agregar(3);

            Debug.WriteLine("Largo tras agregar: " + arr.Largo());
            Debug.WriteLine("Elemento en 1: " + arr.Obtener(1));

            arr.Eliminar(1); // 
            Debug.WriteLine("Largo tras eliminar posición 1: " + arr.Largo());

            arr.Limpiar();
            Debug.WriteLine("Largo tras limpiar: " + arr.Largo());

            Debug.WriteLine("=== FIN TEST ARRAY ===");
        }
    }
}
