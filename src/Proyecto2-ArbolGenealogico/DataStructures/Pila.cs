using System;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class Pila<T>
    {
        private NodoPila<T> tope;
        private int cantidad;

        private class NodoPila<TNodo>
        {
            public TNodo Valor;
            public NodoPila<TNodo> Siguiente;

            public NodoPila(TNodo valor)
            {
                Valor = valor;
                Siguiente = null;
            }
        }

        public Pila()
        {
            tope = null;
            cantidad = 0;
        }

        // Apila un elemento (push) en el tope
        public void Apilar(T valor)
        {
            NodoPila<T> nuevo = new NodoPila<T>(valor);
            nuevo.Siguiente = tope;
            tope = nuevo;
            cantidad++;
        }

        // Desapila (pop) el elemento superior y lo devuelve
        public T Desapilar()
        {
            if (tope == null)
                throw new InvalidOperationException("La pila está vacía.");
            T valor = tope.Valor;
            tope = tope.Siguiente;
            cantidad--;
            return valor;
        }

        // Observa el elemento en el tope sin quitarlo
        public T VerTope()
        {
            if (tope == null)
                throw new InvalidOperationException("La pila está vacía.");
            return tope.Valor;
        }

        // Devuelve la cantidad de elementos en la pila
        public int Largo() => cantidad;

        public bool EstaVacia() => cantidad == 0;

        // Vacía toda la pila
        public void Limpiar()
        {
            tope = null;
            cantidad = 0;
        }
    }
}
