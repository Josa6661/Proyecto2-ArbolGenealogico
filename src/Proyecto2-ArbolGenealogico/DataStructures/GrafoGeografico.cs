namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class GrafoGeografico
    {
        private Dictionary<string, NodoGrafo> nodos; // cedula -> nodo
        private Dictionary<string, Dictionary<string, double>> distancias; // matriz de adyacencia

        public class NodoGrafo
        {
            public string Cedula;
            public string Nombre;
            public double Latitud;
            public double Longitud;
            public string FotoRuta;

            public NodoGrafo(string cedula, string nombre, double lat, double lon, string foto)
            {
                Cedula = cedula;
                Nombre = nombre;
                Latitud = lat;
                Longitud = lon;
                FotoRuta = foto;
            }
        }

        public GrafoGeografico()
        {
            nodos = new Dictionary<string, NodoGrafo>();
            distancias = new Dictionary<string, Dictionary<string, double>>();
        }

        // Agrega un nodo al grafo
        public void AgregarNodo(NodoGrafo nodo)
        {
            if (!nodos.ContainsKey(nodo.Cedula))
            {
                nodos[nodo.Cedula] = nodo;
                distancias[nodo.Cedula] = new Dictionary<string, double>();
            }
        }

        // Calcula y almacena distancia entre dos nodos
        public void CalcularYGuardarDistancia(string cedula1, string cedula2)
        {
            if (!nodos.ContainsKey(cedula1) || !nodos.ContainsKey(cedula2))
                return;

            var nodo1 = nodos[cedula1];
            var nodo2 = nodos[cedula2];

            double distancia = CalcularDistanciaHaversine(
                nodo1.Latitud, nodo1.Longitud,
                nodo2.Latitud, nodo2.Longitud
            );

            // Grafo no dirigido (bidireccional)
            distancias[cedula1][cedula2] = distancia;
            distancias[cedula2][cedula1] = distancia;
        }

        // Fórmula de Haversine para distancia geográfica
        private double CalcularDistanciaHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radio de la Tierra en km

            double dLat = ToRadianes(lat2 - lat1);
            double dLon = ToRadianes(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadianes(lat1)) * Math.Cos(ToRadianes(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadianes(double grados)
        {
            return grados * Math.PI / 180;
        }

        // Obtiene distancia entre dos nodos
        public double ObtenerDistancia(string cedula1, string cedula2)
        {
            if (distancias.ContainsKey(cedula1) && distancias[cedula1].ContainsKey(cedula2))
                return distancias[cedula1][cedula2];
            return -1; // No calculada
        }

        // Obtiene todas las distancias desde un nodo específico
        public Dictionary<string, double> ObtenerDistanciasDesde(string cedula)
        {
            if (distancias.ContainsKey(cedula))
                return new Dictionary<string, double>(distancias[cedula]);
            return new Dictionary<string, double>();
        }

        // Obtiene todos los nodos
        public List<NodoGrafo> ObtenerTodosNodos()
        {
            return new List<NodoGrafo>(nodos.Values);
        }

        // Recalcula TODAS las distancias (llamar después de agregar nodos)
        public void RecalcularTodasDistancias()
        {
            var listaNodos = new List<string>(nodos.Keys);

            for (int i = 0; i < listaNodos.Count; i++)
            {
                for (int j = i + 1; j < listaNodos.Count; j++)
                {
                    CalcularYGuardarDistancia(listaNodos[i], listaNodos[j]);
                }
            }
        }

        // ESTADÍSTICAS REQUERIDAS
        public (string cedula1, string cedula2, double distancia) ObtenerParMasLejano()
        {
            // Validación
            if (nodos.Count < 2)
                return ("", "", 0);

            double maxDistancia = 0;
            string ced1 = "", ced2 = "";

            foreach (var kvp1 in distancias)
            {
                foreach (var kvp2 in kvp1.Value)
                {
                    if (kvp2.Value > maxDistancia)
                    {
                        maxDistancia = kvp2.Value;
                        ced1 = kvp1.Key;
                        ced2 = kvp2.Key;
                    }
                }
            }

            return (ced1, ced2, maxDistancia);
        }

        public (string cedula1, string cedula2, double distancia) ObtenerParMasCercano()
        {
            // Validación
            if (nodos.Count < 2)
                return ("", "", 0);

            double minDistancia = double.MaxValue;
            string ced1 = "", ced2 = "";

            foreach (var kvp1 in distancias)
            {
                foreach (var kvp2 in kvp1.Value)
                {
                    if (kvp2.Value < minDistancia && kvp2.Value > 0)
                    {
                        minDistancia = kvp2.Value;
                        ced1 = kvp1.Key;
                        ced2 = kvp2.Key;
                    }
                }
            }

            return (ced1, ced2, minDistancia);
        }

        public double ObtenerDistanciaPromedio()
        {
            double suma = 0;
            int contador = 0;

            foreach (var kvp1 in distancias)
            {
                foreach (var kvp2 in kvp1.Value)
                {
                    suma += kvp2.Value;
                    contador++;
                }
            }

            return contador > 0 ? suma / contador : 0;
        }
    }
}


