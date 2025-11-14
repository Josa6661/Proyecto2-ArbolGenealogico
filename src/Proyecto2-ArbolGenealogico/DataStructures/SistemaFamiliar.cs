using System.Linq;
namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class SistemaFamiliar
    {
        public ArbolGenealogico Arbol { get; private set; }
        public GrafoGeografico Grafo { get; private set; }

        public SistemaFamiliar()
        {
            Arbol = new ArbolGenealogico();
            Grafo = new GrafoGeografico();
        }
        
        // Reconstruir el grafo geográfico desde el árbol
        public void ReconstruirGrafo()
        {
            Grafo.ConstruirDesdeArbol(Arbol);
        }

        public bool AgregarMiembroCompleto(string nombrePadre, NodoFamiliar familiar)
        {
            if (!Arbol.TieneRaiz())
            {
                Arbol.CrearRaiz(familiar);
            }
            else
            {
                if (!Arbol.AgregarMiembro(nombrePadre, familiar))
                    return false;
            }

            var nodoGrafo = new GrafoGeografico.NodoGrafo(
                familiar.Cedula,
                familiar.Nombre,
                familiar.Latitud,
                familiar.Longitud,
                familiar.FotoRuta
            );
            Grafo.AgregarNodo(nodoGrafo);
            Grafo.RecalcularTodasDistancias();
            return true;
        }

        public NodoFamiliar BuscarPorNombre(string nombre)
        {
            return Arbol.BuscarPorNombre(nombre);
        }

        public (string nombre1, string nombre2, double distancia) ParMasLejano()
        {
            var (ced1, ced2, dist) = Grafo.ObtenerParMasLejano();

            // Validar que tengamos cédulas válidas
            if (string.IsNullOrEmpty(ced1) || string.IsNullOrEmpty(ced2))
                return ("", "", 0);

            // Buscar nodos en el grafo
            var nodos = Grafo.ObtenerTodosNodos();
            GrafoGeografico.NodoGrafo nodo1 = null, nodo2 = null;

            for (int i = 0; i < nodos.Largo(); i++)
            {
                var nodo = nodos.Obtener(i);
                if (nodo.Cedula == ced1) nodo1 = nodo;
                if (nodo.Cedula == ced2) nodo2 = nodo;
            }

            return (nodo1?.Nombre ?? "", nodo2?.Nombre ?? "", dist);
        }

        public (string nombre1, string nombre2, double distancia) ParMasCercano()
        {
            var (ced1, ced2, dist) = Grafo.ObtenerParMasCercano();

            if (string.IsNullOrEmpty(ced1) || string.IsNullOrEmpty(ced2))
                return ("", "", 0);

            var nodos = Grafo.ObtenerTodosNodos();
            GrafoGeografico.NodoGrafo nodo1 = null, nodo2 = null;

            for (int i = 0; i < nodos.Largo(); i++)
            {
                var nodo = nodos.Obtener(i);
                if (nodo.Cedula == ced1) nodo1 = nodo;
                if (nodo.Cedula == ced2) nodo2 = nodo;
            }

            return (nodo1?.Nombre ?? "", nodo2?.Nombre ?? "", dist);
        }

        public double ObtenerDistanciaPromedio()
        {
            return Grafo.ObtenerDistanciaPromedio();
        }
    }
}