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

        // Agrega miembro a AMBAS estructuras
        public bool AgregarMiembroCompleto(string nombrePadre, NodoFamiliar familiar)
        {
            // Si es la raíz
            if (!Arbol.TieneRaiz())
            {
                Arbol.CrearRaiz(familiar);
            }
            else
            {
                if (!Arbol.AgregarMiembro(nombrePadre, familiar))
                    return false;
            }

            // Agregar al grafo
            var nodoGrafo = new GrafoGeografico.NodoGrafo(
                familiar.Cedula,
                familiar.Nombre,
                familiar.Latitud,
                familiar.Longitud,
                familiar.FotoRuta
            );
            Grafo.AgregarNodo(nodoGrafo);

            // Recalcular distancias
            Grafo.RecalcularTodasDistancias();

            return true;
        }

        // Búsqueda unificada
        public NodoFamiliar BuscarPorNombre(string nombre)
        {
            return Arbol.BuscarPorNombre(nombre);
        }

        // Obtener estadísticas del grafo
        public (string nombre1, string nombre2, double distancia) ParMasLejano()
        {
            var (ced1, ced2, dist) = Grafo.ObtenerParMasLejano();
            var nodo1 = Arbol.BuscarPorNombre(Grafo.ObtenerTodosNodos()
                .First(n => n.Cedula == ced1).Nombre);
            var nodo2 = Arbol.BuscarPorNombre(Grafo.ObtenerTodosNodos()
                .First(n => n.Cedula == ced2).Nombre);
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