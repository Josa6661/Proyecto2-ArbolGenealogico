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

        // ✅ ARREGLADO: Maneja casos vacíos
        public (string nombre1, string nombre2, double distancia) ParMasLejano()
        {
            var (ced1, ced2, dist) = Grafo.ObtenerParMasLejano();

            // Validar que tengamos cédulas válidas
            if (string.IsNullOrEmpty(ced1) || string.IsNullOrEmpty(ced2))
                return ("", "", 0);

            // Buscar nodos en el grafo usando FirstOrDefault para evitar excepciones
            var nodo1 = Grafo.ObtenerTodosNodos().FirstOrDefault(n => n.Cedula == ced1);
            var nodo2 = Grafo.ObtenerTodosNodos().FirstOrDefault(n => n.Cedula == ced2);

            return (nodo1?.Nombre ?? "", nodo2?.Nombre ?? "", dist);
        }

        public (string nombre1, string nombre2, double distancia) ParMasCercano()
        {
            var (ced1, ced2, dist) = Grafo.ObtenerParMasCercano();

            if (string.IsNullOrEmpty(ced1) || string.IsNullOrEmpty(ced2))
                return ("", "", 0);

            var nodo1 = Grafo.ObtenerTodosNodos().FirstOrDefault(n => n.Cedula == ced1);
            var nodo2 = Grafo.ObtenerTodosNodos().FirstOrDefault(n => n.Cedula == ced2);

            return (nodo1?.Nombre ?? "", nodo2?.Nombre ?? "", dist);
        }

        public double ObtenerDistanciaPromedio()
        {
            return Grafo.ObtenerDistanciaPromedio();
        }
    }
}