using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class ArbolGenealogico
    {
        // Propiedad de solo lectura (encapsulada)
        public NodoFamiliar Raiz { get; private set; }

        // Constructor
        public ArbolGenealogico()
        {
            Raiz = null;
        }

        // Define el primer nodo/familiar como raíz
        public void CrearRaiz(NodoFamiliar miembro)
        {
            Raiz = miembro;
        }

        // Busca un familiar por nombre en todo el árbol
        public NodoFamiliar BuscarPorNombre(string nombre)
        {
            return BuscarPorNombreRecursivo(Raiz, nombre);
        }

        // Implementación recursiva de búsqueda
        private NodoFamiliar BuscarPorNombreRecursivo(NodoFamiliar actual, string nombre)
        {
            if (actual == null)
                return null;

            if (actual.Nombre == nombre)
                return actual;

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                var encontrado = BuscarPorNombreRecursivo(actual.Hijos.Obtener(i), nombre);
                if (encontrado != null)
                    return encontrado;
            }

            return null;
        }

        // Agrega un hijo a un miembro existente (busca el padre por nombre)
        public bool AgregarMiembro(string nombrePadre, NodoFamiliar hijo)
        {
            var padre = BuscarPorNombre(nombrePadre);
            if (padre != null)
            {
                padre.AgregarHijo(hijo);
                hijo.Padre = padre;
                return true;
            }
            return false;
        }

        // Elimina un miembro por nombre (y su sub-árbol)
        public bool EliminarMiembro(string nombre)
        {
            // Si la raíz es el nodo a eliminar, borra todo el árbol
            if (Raiz != null && Raiz.Nombre == nombre)
            {
                Raiz = null;
                return true;
            }

            // Busca en subárboles
            return EliminarRecursivo(Raiz, nombre);
        }

        private bool EliminarRecursivo(NodoFamiliar actual, string nombre)
        {
            if (actual == null)
                return false;

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                var hijo = actual.Hijos.Obtener(i);

                if (hijo.Nombre == nombre)
                {
                    actual.Hijos.EliminarPorIndice(i);
                    return true;
                }

                if (EliminarRecursivo(hijo, nombre))
                    return true;
            }

            return false;
        }

        // Limpia completamente el árbol
        public void Limpiar()
        {
            Raiz = null;
        }

        // Retorna todos los nodos del árbol (para visualización o guardado)
        public List<NodoFamiliar> ObtenerTodos()
        {
            var lista = new List<NodoFamiliar>();
            ObtenerTodosRecursivo(Raiz, lista);
            return lista;
        }

        private void ObtenerTodosRecursivo(NodoFamiliar actual, List<NodoFamiliar> lista)
        {
            if (actual == null)
                return;

            lista.Add(actual);

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                ObtenerTodosRecursivo(actual.Hijos.Obtener(i), lista);
            }
        }
    }
}
