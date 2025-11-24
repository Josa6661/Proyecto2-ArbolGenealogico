using Proyecto2_ArbolGenealogico.Services;
using Proyecto2_ArbolGenealogico.BusinessLogic;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class GrafoGeografico
    {
        private ListaEnlazada<NodoGrafo> nodos;
        private ListaEnlazada<DistanciaEntreNodos> distancias;

        // Clase auxiliar para almacenar distancias
        private sealed class DistanciaEntreNodos
        {
            public string Cedula1 { get; set; }
            public string Cedula2 { get; set; }
            public double Distancia { get; set; }

            public DistanciaEntreNodos(string ced1, string ced2, double dist)
            {
                Cedula1 = ced1;
                Cedula2 = ced2;
                Distancia = dist;
            }
        }

        public class NodoGrafo
        {
            public string Cedula { get; set;}
            public string Nombre { get; set; }
            public double Latitud { get; set; }
            public double Longitud { get; set; }
            public string FotoRuta { get; set; }
            public NodoFamiliar MiembroFamiliar { get; set; }

            public NodoGrafo(string cedula, string nombre, double lat, double lon, string foto)
            {
                Cedula = cedula;
                Nombre = nombre;
                Latitud = lat;
                Longitud = lon;
                FotoRuta = foto;
            }

            public NodoGrafo(NodoFamiliar miembro)
            {
                MiembroFamiliar = miembro;
                Cedula = miembro.Cedula;
                Nombre = miembro.Nombre;
                Latitud = miembro.Latitud;
                Longitud = miembro.Longitud;
                FotoRuta = miembro.FotoRuta;
            }
        }

        public GrafoGeografico()
        {
            nodos = new ListaEnlazada<NodoGrafo>();
            distancias = new ListaEnlazada<DistanciaEntreNodos>();
        }

        // Construir el grafo desde un árbol genealógico
        public void ConstruirDesdeArbol(ArbolGenealogico arbol)
        {
            nodos = new ListaEnlazada<NodoGrafo>();
            distancias = new ListaEnlazada<DistanciaEntreNodos>();

            if (!arbol.TieneRaiz())
                return;

            var todosLosMiembros = arbol.ObtenerTodos();
            
            for (int i = 0; i < todosLosMiembros.Largo(); i++)
            {
                var miembro = todosLosMiembros.Obtener(i);
                var nodo = new NodoGrafo(miembro);
                AgregarNodo(nodo);
            }

            // Calcular todas las distancias
            RecalcularTodasDistancias();
        }

        // Agrega un nodo al grafo
        public void AgregarNodo(NodoGrafo nodo)
        {
            // Verificar si ya existe
            bool existe = false;
            for (int i = 0; i < nodos.Largo(); i++)
            {
                if (nodos.Obtener(i).Cedula == nodo.Cedula)
                {
                    existe = true;
                    break;
                }
            }

            if (!existe)
            {
                nodos.AgregarFinal(nodo);
            }
        }

        // Elimina un nodo del grafo
        public bool EliminarNodo(string cedula)
        {
            for (int i = 0; i < nodos.Largo(); i++)
            {
                if (nodos.Obtener(i).Cedula == cedula)
                {
                    nodos.EliminarPorIndice(i);
                    
                    // Eliminar todas las distancias relacionadas con este nodo
                    for (int j = distancias.Largo() - 1; j >= 0; j--)
                    {
                        var dist = distancias.Obtener(j);
                        if (dist.Cedula1 == cedula || dist.Cedula2 == cedula)
                        {
                            distancias.EliminarPorIndice(j);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        // Buscar nodo por cédula
        private NodoGrafo BuscarNodo(string cedula)
        {
            for (int i = 0; i < nodos.Largo(); i++)
            {
                var nodo = nodos.Obtener(i);
                if (nodo.Cedula == cedula)
                    return nodo;
            }
            return null;
        }

        // Buscar nodo por nombre
        public NodoGrafo BuscarPorNombre(string nombre)
        {
            for (int i = 0; i < nodos.Largo(); i++)
            {
                var nodo = nodos.Obtener(i);
                if (nodo.Nombre == nombre)
                    return nodo;
            }
            return null;
        }

        // Calcula y almacena distancia entre dos nodos
        public void CalcularYGuardarDistancia(string cedula1, string cedula2)
        {
            var nodo1 = BuscarNodo(cedula1);
            var nodo2 = BuscarNodo(cedula2);

            if (nodo1 == null || nodo2 == null)
                return;

            double distancia = CalculadoraDistancias.CalcularDistanciaHaversine(
                nodo1.Latitud, nodo1.Longitud,
                nodo2.Latitud, nodo2.Longitud
            );

            // Agregar distancia
            distancias.AgregarFinal(new DistanciaEntreNodos(cedula1, cedula2, distancia));
        }

        // Obtiene distancia entre dos nodos
        public double ObtenerDistancia(string cedula1, string cedula2)
        {
            for (int i = 0; i < distancias.Largo(); i++)
            {
                var dist = distancias.Obtener(i);
                if ((dist.Cedula1 == cedula1 && dist.Cedula2 == cedula2) ||
                    (dist.Cedula1 == cedula2 && dist.Cedula2 == cedula1))
                {
                    return dist.Distancia;
                }
            }
            return -1; // No calculada
        }

        // Obtiene todas las distancias desde un nodo específico
        public ListaEnlazada<(string cedula, double distancia)> ObtenerDistanciasDesde(string cedula)
        {
            var resultado = new ListaEnlazada<(string, double)>();
            
            for (int i = 0; i < distancias.Largo(); i++)
            {
                var dist = distancias.Obtener(i);
                if (dist.Cedula1 == cedula)
                {
                    resultado.AgregarFinal((dist.Cedula2, dist.Distancia));
                }
                else if (dist.Cedula2 == cedula)
                {
                    resultado.AgregarFinal((dist.Cedula1, dist.Distancia));
                }
            }
            
            return resultado;
        }

        // Obtiene todos los nodos
        public ListaEnlazada<NodoGrafo> ObtenerTodosNodos()
        {
            return nodos;
        }

        // Recalcula TODAS las distancias (llamar después de agregar nodos)
        public void RecalcularTodasDistancias()
        {
            distancias = new ListaEnlazada<DistanciaEntreNodos>(); // Limpiar distancias anteriores

            for (int i = 0; i < nodos.Largo(); i++)
            {
                for (int j = i + 1; j < nodos.Largo(); j++)
                {
                    CalcularYGuardarDistancia(nodos.Obtener(i).Cedula, nodos.Obtener(j).Cedula);
                }
            }
        }

        // ESTADÍSTICAS REQUERIDAS
        public (string cedula1, string cedula2, double distancia) ObtenerParMasLejano()
        {
            // Validación
            if (nodos.Largo() < 2)
                return ("", "", 0);

            double maxDistancia = 0;
            string ced1 = "", ced2 = "";

            for (int i = 0; i < distancias.Largo(); i++)
            {
                var dist = distancias.Obtener(i);
                if (dist.Distancia > maxDistancia)
                {
                    maxDistancia = dist.Distancia;
                    ced1 = dist.Cedula1;
                    ced2 = dist.Cedula2;
                }
            }

            return (ced1, ced2, maxDistancia);
        }

        public (string cedula1, string cedula2, double distancia) ObtenerParMasCercano()
        {
            // Validación
            if (nodos.Largo() < 2)
                return ("", "", 0);

            double minDistancia = double.MaxValue;
            string ced1 = "", ced2 = "";

            for (int i = 0; i < distancias.Largo(); i++)
            {
                var dist = distancias.Obtener(i);
                if (dist.Distancia < minDistancia && dist.Distancia > 0)
                {
                    minDistancia = dist.Distancia;
                    ced1 = dist.Cedula1;
                    ced2 = dist.Cedula2;
                }
            }

            return (ced1, ced2, minDistancia);
        }

        public double ObtenerDistanciaPromedio()
        {
            if (distancias.Largo() == 0)
                return 0;

            double suma = 0;
            int contador = distancias.Largo();

            for (int i = 0; i < contador; i++)
            {
                suma += distancias.Obtener(i).Distancia;
            }

            return contador > 0 ? suma / contador : 0;
        }

        // MÉTODOS DE RECORRIDO
        // Recorrido en amplitud (BFS) desde un nodo
        public ListaEnlazada<NodoGrafo> RecorridoBFS(string cedulaInicio)
        {
            var resultado = new ListaEnlazada<NodoGrafo>();
            var nodoInicio = BuscarNodo(cedulaInicio);

            if (nodoInicio == null)
                return resultado;

            var visitados = new ListaEnlazada<string>();
            var cola = new Cola<NodoGrafo>();

            cola.Encolar(nodoInicio);
            visitados.AgregarFinal(nodoInicio.Cedula);

            while (!cola.EstaVacia())
            {
                var nodoActual = cola.Desencolar();
                resultado.AgregarFinal(nodoActual);

                // Agregar nodos vecinos a la cola
                // En un grafo completo de residencias, todos están conectados
                for (int i = 0; i < nodos.Largo(); i++)
                {
                    var vecino = nodos.Obtener(i);
                    if (!visitados.Contiene(vecino.Cedula))
                    {
                        visitados.AgregarFinal(vecino.Cedula);
                        cola.Encolar(vecino);
                    }
                }
            }

            return resultado;
        }

        // Recorrido en profundidad (DFS) desde un nodo
        public ListaEnlazada<NodoGrafo> RecorridoDFS(string cedulaInicio)
        {
            var resultado = new ListaEnlazada<NodoGrafo>();
            var visitados = new ListaEnlazada<string>();
            var nodoInicio = BuscarNodo(cedulaInicio);

            if (nodoInicio == null)
                return resultado;

            RecorridoDFSRecursivo(nodoInicio, visitados, resultado);
            return resultado;
        }

        private void RecorridoDFSRecursivo(NodoGrafo nodo, ListaEnlazada<string> visitados, ListaEnlazada<NodoGrafo> resultado)
        {
            visitados.AgregarFinal(nodo.Cedula);
            resultado.AgregarFinal(nodo);

            // Visitar nodos no visitados
            for (int i = 0; i < nodos.Largo(); i++)
            {
                var vecino = nodos.Obtener(i);
                if (!visitados.Contiene(vecino.Cedula))
                {
                    RecorridoDFSRecursivo(vecino, visitados, resultado);
                }
            }
        }

        // MÉTODOS AUXILIARES
        public int CantidadNodos()
        {
            return nodos.Largo();
        }

        public bool EstaVacio()
        {
            return nodos.Largo() == 0;
        }
    }
}


