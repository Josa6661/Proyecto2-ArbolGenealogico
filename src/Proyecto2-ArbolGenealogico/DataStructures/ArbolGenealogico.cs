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

            // Buscar en el cónyuge
            if (actual.Conyuge != null && actual.Conyuge.Nombre == nombre)
                return actual.Conyuge;

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

        public ListaEnlazada<NodoFamiliar> ObtenerTodos()
        {
            var lista = new ListaEnlazada<NodoFamiliar>();
            ObtenerTodosRecursivo(Raiz, lista);
            return lista;
        }

        // Obtiene solo los nodos en la jerarquía del árbol (sin cónyuges)
        public ListaEnlazada<NodoFamiliar> ObtenerNodosJerarquicos()
        {
            var lista = new ListaEnlazada<NodoFamiliar>();
            ObtenerNodosJerarquicosRecursivo(Raiz, lista);
            return lista;
        }

        private void ObtenerNodosJerarquicosRecursivo(NodoFamiliar actual, ListaEnlazada<NodoFamiliar> lista)
        {
            if (actual == null)
                return;

            lista.AgregarFinal(actual);

            // NO agregar el cónyuge, solo los descendientes
            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                ObtenerNodosJerarquicosRecursivo(actual.Hijos.Obtener(i), lista);
            }
        }

        private void ObtenerTodosRecursivo(NodoFamiliar actual, ListaEnlazada<NodoFamiliar> lista)
        {
            if (actual == null)
                return;

            lista.AgregarFinal(actual);

            // Agregar el cónyuge si existe y no está ya en la lista
            if (actual.Conyuge != null && !lista.Contiene(actual.Conyuge))
            {
                lista.AgregarFinal(actual.Conyuge);
            }

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                ObtenerTodosRecursivo(actual.Hijos.Obtener(i), lista);
            }
        }

        // Buscar por cédula para evitar duplicados
        public NodoFamiliar BuscarPorCedula(string cedula)
        {
            return BuscarPorCedulaRecursivo(Raiz, cedula);
        }

        private NodoFamiliar BuscarPorCedulaRecursivo(NodoFamiliar actual, string cedula)
        {
            if (actual == null)
                return null;

            if (actual.Cedula == cedula)
                return actual;

            // Buscar en el cónyuge
            if (actual.Conyuge != null && actual.Conyuge.Cedula == cedula)
                return actual.Conyuge;

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                var encontrado = BuscarPorCedulaRecursivo(actual.Hijos.Obtener(i), cedula);
                if (encontrado != null)
                    return encontrado;
            }

            return null;
        }

        // Agregar un padre a la raíz actual (convierte la raíz actual en hijo del nuevo padre)
        public void AgregarPadreARaiz(NodoFamiliar nuevoPadre)
        {
            if (Raiz != null)
            {
                // La raíz actual se convierte en hijo del nuevo padre
                nuevoPadre.AgregarHijo(Raiz);
            }
            // El nuevo padre se convierte en la nueva raíz
            Raiz = nuevoPadre;
        }

        // Agregar cónyuge a un miembro existente
        public bool AgregarConyuge(string nombreMiembro, NodoFamiliar conyuge)
        {
            var miembro = BuscarPorNombre(nombreMiembro);
            if (miembro == null)
                return false;

            if (miembro.Conyuge != null)
                return false; // Ya tiene cónyuge

            miembro.EstablecerConyuge(conyuge);
            return true;
        }
    }
}