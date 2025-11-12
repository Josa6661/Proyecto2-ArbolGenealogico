using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class ArbolGenealogico
    {
        public NodoFamiliar Raiz { get; private set; }

        public ArbolGenealogico()
        {
            Raiz = null;
        }

        public void CrearRaiz(NodoFamiliar miembro)
        {
            Raiz = miembro;
        }

        public bool TieneRaiz()
        {
            return Raiz != null;
        }

        public NodoFamiliar BuscarPorNombre(string nombre)
        {
            return BuscarPorNombreRecursivo(Raiz, nombre);
        }

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

        public bool AgregarMiembro(string nombrePadre, NodoFamiliar hijo)
        {
            // Validación de raíz
            if (Raiz == null)
                return false;

            var padre = BuscarPorNombre(nombrePadre);
            if (padre != null)
            {
                padre.AgregarHijo(hijo);
                return true;
            }
            return false;
        }

        public bool EliminarMiembro(string nombre)
        {
            if (Raiz != null && Raiz.Nombre == nombre)
            {
                Raiz = null;
                return true;
            }

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

        public void Limpiar()
        {
            Raiz = null;
        }

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