using System;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class Array<T>
    {
        private T[] elementos;
        private int cantidad;
        private int capacidad;

        // Constructor
        public Array(int capacidadInicial = 10)
        {
            capacidad = capacidadInicial;
            elementos = new T[capacidad];
            cantidad = 0;
        }

        // Devuelve cantidad de elementos
        public int Largo() => cantidad;

        // Agrega elemento al final
        public void Agregar(T elemento)
        {
            if (cantidad == capacidad)
                Ampliar(); // duplica la capacidad si se llena
            elementos[cantidad] = elemento;
            cantidad++;
        }

        // Obtiene elemento por índice
        public T Obtener(int indice)
        {
            if (indice < 0 || indice >= cantidad)
                throw new ArgumentOutOfRangeException("Índice fuera de rango.");
            return elementos[indice];
        }

        // Duplica el tamaño del arreglo
        private void Ampliar()
        {
            capacidad = capacidad * 2;
            T[] nuevo = new T[capacidad];
            for (int i = 0; i < cantidad; i++)
                nuevo[i] = elementos[i];
            elementos = nuevo;
        }

        // Elimina elemento por índice y recorre los siguientes
        public void EliminarPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
                throw new ArgumentOutOfRangeException("Índice fuera de rango.");
            for (int i = indice; i < cantidad - 1; i++)
                elementos[i] = elementos[i + 1];
            cantidad--;
        }

        // Vacía el array
        public void Limpiar()
        {
            elementos = new T[capacidad];
            cantidad = 0;
        }
    }
}
