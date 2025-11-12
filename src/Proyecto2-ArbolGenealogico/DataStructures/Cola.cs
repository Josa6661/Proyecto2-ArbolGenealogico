using System;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class Cola<T>
    {
        private NodoCola<T> frente;
        private NodoCola<T> final;
        private int cantidad;

        private class NodoCola<TNodo>
        {
            public TNodo Valor;
            public NodoCola<TNodo> Siguiente;

            public NodoCola(TNodo valor)
            {
                Valor = valor;
                Siguiente = null;
            }
        }

        public Cola()
        {
            frente = null;
            final = null;
            cantidad = 0;
        }

        // Encola (enqueue) un elemento al final
        public void Encolar(T valor)
        {
            NodoCola<T> nuevo = new NodoCola<T>(valor);
            if (final == null)
            {
                frente = nuevo;
                final = nuevo;
            }
            else
            {
                final.Siguiente = nuevo;
                final = nuevo;
            }
            cantidad++;
        }

        // Desencola (dequeue) y devuelve el elemento al frente
        public T Desencolar()
        {
            if (frente == null)
                throw new InvalidOperationException("La cola está vacía.");
            T valor = frente.Valor;
            frente = frente.Siguiente;
            if (frente == null)
                final = null;
            cantidad--;
            return valor;
        }

        // Observa el elemento al frente sin quitarlo
        public T VerFrente()
        {
            if (frente == null)
                throw new InvalidOperationException("La cola está vacía.");
            return frente.Valor;
        }

        // Devuelve la cantidad de elementos en la cola
        public int Largo() => cantidad;

        public bool EstaVacia() => cantidad == 0;

        // Vacía toda la cola
        public void Limpiar()
        {
            frente = null;
            final = null;
            cantidad = 0;
        }
    }
}
