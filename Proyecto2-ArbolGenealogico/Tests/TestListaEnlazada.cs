using Proyecto2_ArbolGenealogico.DataStructures;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestListaEnlazada
    {
        public static void ProbarLista()
        {
            Debug.WriteLine("=== INICIO TEST LISTA ENLAZADA ===");

            var lista = new ListaEnlazada<int>();

            // Agregar elementos
            lista.AgregarInicio(30);
            lista.AgregarFinal(40);
            lista.AgregarFinal(50);
            lista.AgregarInicio(20);

            Debug.WriteLine("Largo tras agregar: " + lista.Largo());

            // Obtener por índice
            Debug.WriteLine("Elemento en 0: " + lista.Obtener(0));
            Debug.WriteLine("Elemento en 3: " + lista.Obtener(3));

            // Buscar por valor
            Debug.WriteLine("Índice de 30: " + lista.Buscar(30));
            Debug.WriteLine("Índice de 99: " + lista.Buscar(99));

            // Eliminar por valor
            lista.EliminarPorValor(40);
            Debug.WriteLine("Largo tras eliminar 40: " + lista.Largo());

            // Eliminar por índice
            lista.EliminarPorIndice(0);
            Debug.WriteLine("Largo tras eliminar por índice 0: " + lista.Largo());

            // Limpiar lista
            lista.Limpiar();
            Debug.WriteLine("Largo tras limpiar: " + lista.Largo());

            Debug.WriteLine("=== FIN TEST LISTA ENLAZADA ===");
        }
    }
}
