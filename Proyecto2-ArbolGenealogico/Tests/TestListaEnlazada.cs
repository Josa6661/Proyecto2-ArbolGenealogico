using Proyecto2_ArbolGenealogico.DataStructures;
using System;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class TestListaEnlazada
    {
        public static void ProbarLista()
        {
            Console.WriteLine("=== INICIO TEST LISTA ENLAZADA ===");

            var lista = new ListaEnlazada<int>();

            // Agregar elementos
            lista.AgregarInicio(30);
            lista.AgregarFinal(40);
            lista.AgregarFinal(50);
            lista.AgregarInicio(20);

            Console.WriteLine("Largo tras agregar: " + lista.Largo());

            // Obtener por índice
            Console.WriteLine("Elemento en 0: " + lista.Obtener(0));
            Console.WriteLine("Elemento en 3: " + lista.Obtener(3));

            // Buscar por valor
            Console.WriteLine("Índice de 30: " + lista.Buscar(30));
            Console.WriteLine("Índice de 99: " + lista.Buscar(99));

            // Eliminar por valor
            lista.EliminarPorValor(40);
            Console.WriteLine("Largo tras eliminar 40: " + lista.Largo());

            // Eliminar por índice
            lista.EliminarPorIndice(0);
            Console.WriteLine("Largo tras eliminar por índice 0: " + lista.Largo());

            // Limpiar lista
            lista.Limpiar();
            Console.WriteLine("Largo tras limpiar: " + lista.Largo());

            Console.WriteLine("=== FIN TEST LISTA ENLAZADA ===");
        }
    }
}
