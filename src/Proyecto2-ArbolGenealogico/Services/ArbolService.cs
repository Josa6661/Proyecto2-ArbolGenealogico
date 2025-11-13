using Proyecto2_ArbolGenealogico.DataStructures;
namespace Proyecto2_ArbolGenealogico.Services
{
public class ArbolService
    {
        private static ArbolService _instancia;
        public ArbolGenealogico Arbol { get; private set; }

        // Singleton para tener una sola instancia en toda la app
        public static ArbolService Instancia
        {
            get
            {
                if (_instancia == null)
                    _instancia = new ArbolService();
                return _instancia;
            }
        }

        private ArbolService()
        {
            Arbol = new ArbolGenealogico();
        }
        public bool CrearRaiz(NodoFamiliar nodo)
        {
            if (Arbol.Raiz != null)
                return false; // Ya existe raíz
            Arbol.CrearRaiz(nodo);
            return true;
        }
        public bool AgregarMiembro(string nombrePadre, NodoFamiliar hijo)
        {
            return Arbol.AgregarMiembro(nombrePadre, hijo);
        }
        public NodoFamiliar BuscarMiembro(string nombre)
        {
            return Arbol.BuscarPorNombre(nombre);
        }
        public bool EliminarMiembro(string nombre)
        {
            return Arbol.EliminarMiembro(nombre);
        }
        public void Limpiar()
        {
            Arbol.Limpiar();
        }
        public ListaEnlazada<NodoFamiliar> ObtenerTodos()
        {
            return Arbol.ObtenerTodos();
        }
     }
}

