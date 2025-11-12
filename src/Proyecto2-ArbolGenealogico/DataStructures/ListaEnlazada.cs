namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class NodoLista<T>
    {
        public T Valor { get; set; }
        public NodoLista<T> Siguiente { get; set; }

        public NodoLista(T valor)
        {
            Valor = valor;
            Siguiente = null;
        }
    }

    public class ListaEnlazada<T>
    {
        private NodoLista<T> cabeza;
        private int cantidad;

        public ListaEnlazada()
        {
            cabeza = null;
            cantidad = 0;
        }

        // Agrega al inicio
        public void AgregarInicio(T valor)
        {
            NodoLista<T> nuevo = new NodoLista<T>(valor);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
            cantidad++;
        }

        // Agrega al final
        public void AgregarFinal(T valor)
        {
            NodoLista<T> nuevo = new NodoLista<T>(valor);
            if (cabeza == null)
                cabeza = nuevo;
            else
            {
                NodoLista<T> actual = cabeza;
                while (actual.Siguiente != null)
                    actual = actual.Siguiente;
                actual.Siguiente = nuevo;
            }
            cantidad++;
        }

        // Elimina primer nodo que tenga el valor indicado
        public void EliminarPorValor(T valor)
        {
            NodoLista<T> previo = null;
            NodoLista<T> actual = cabeza;
            while (actual != null && !actual.Valor.Equals(valor))
            {
                previo = actual;
                actual = actual.Siguiente;
            }
            if (actual != null)
            {
                if (previo == null)
                    cabeza = actual.Siguiente;
                else
                    previo.Siguiente = actual.Siguiente;
                cantidad--;
            }
        }

        // Elimina nodo por índice
        public void EliminarPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidad)
                throw new ArgumentOutOfRangeException(nameof(indice), "Índice fuera de rango.");
            NodoLista<T> previo = null;
            NodoLista<T> actual = cabeza;
            for (int i = 0; i < indice; i++)
            {
                previo = actual;
                actual = actual.Siguiente;
            }
            if (previo == null)
                cabeza = actual.Siguiente;
            else
                previo.Siguiente = actual.Siguiente;
            cantidad--;
        }

        // Busca índice de un valor
        public int Buscar(T valor)
        {
            NodoLista<T> actual = cabeza;
            int indice = 0;
            while (actual != null)
            {
                if (actual.Valor.Equals(valor))
                    return indice;
                actual = actual.Siguiente;
                indice++;
            }
            return -1; // no encontrado
        }

        // Vacía la lista
        public void Limpiar()
        {
            cabeza = null;
            cantidad = 0;
        }

        // Obtiene elemento por índice
        public T Obtener(int indice)
        {
            if (indice < 0 || indice >= cantidad)
                throw new ArgumentOutOfRangeException(nameof(indice), "Índice fuera de rango.");
            NodoLista<T> actual = cabeza;
            for (int i = 0; i < indice; i++)
                actual = actual.Siguiente;
            return actual.Valor;
        }

        // Devuelve cantidad de nodos
        public int Largo() => cantidad;
    }
}
