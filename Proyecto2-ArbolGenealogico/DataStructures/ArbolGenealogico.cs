using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class ArbolGenealogico
    {
        // Nodo raíz del árbol
        public NodoFamiliar? Raiz;

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

        // Busca a un familiar por nombre en todo el árbol
        public NodoFamiliar? BuscarPorNombre(string nombre)
        {
            return BuscarPorNombreRecursivo(Raiz, nombre);
        }

        // Implementación recursiva de búsqueda por nombre
        private NodoFamiliar? BuscarPorNombreRecursivo(NodoFamiliar? actual, string nombre)
        {
            if (actual == null)
                return null;
            if (actual.Nombre == nombre)
                return actual;

            // Recorre toda la rama de hijos
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
                return true;
            }
            return false;
        }

        // Elimina a un miembro por nombre (y su sub-árbol), actualiza el padre para quitarlo de la lista de hijos
        public bool EliminarMiembro(string nombre)
        {
            // Si la raíz es el que debe ser eliminado, borra todo el árbol
            if (Raiz != null && Raiz.Nombre == nombre)
            {
                Raiz = null;
                return true;
            }
            // Busca y elimina en subárboles
            return EliminarRecursivo(Raiz, nombre);
        }

        // Recorrida recursiva para encontrar y eliminar el nodo deseado
        private bool EliminarRecursivo(NodoFamiliar? actual, string nombre)
        {
            if (actual == null)
                return false;
            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                if (actual.Hijos.Obtener(i).Nombre == nombre)
                {
                    actual.Hijos.EliminarPorIndice(i);
                    return true;
                }
                if (EliminarRecursivo(actual.Hijos.Obtener(i), nombre))
                    return true;
            }
            return false;
        }

        // Vacía completamente el árbol genealógico
        public void Limpiar()
        {
            Raiz = null;
        }
    }
}
