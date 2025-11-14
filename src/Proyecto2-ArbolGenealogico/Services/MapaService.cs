using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Wpf;
using Mapsui.Utilities;
using NetTopologySuite.Geometries;
using Proyecto2_ArbolGenealogico.DataStructures;
using System.IO;

namespace Proyecto2_ArbolGenealogico.Services
{
    public class MapaService
    {
        private readonly MapControl mapControl;

        public MapaService(MapControl mapControl)
        {
            this.mapControl = mapControl;
        }

        // Configuración inicial del mapa con OpenStreetMap
        public void ConfigurarMapa()
        {
            // Configurar el mapa con OpenStreetMap
            mapControl.Map = new Map
            {
                CRS = "EPSG:3857"
            };

            // Agregar capa de mapa base de OpenStreetMap
            var tileLayer = OpenStreetMap.CreateTileLayer();
            mapControl.Map.Layers.Add(tileLayer);

            // Centrar el mapa en una posición inicial (0, 0 - África ecuatorial)
            var centerPoint = new MPoint(0, 0);
            mapControl.Map.Navigator.CenterOn(centerPoint);
            mapControl.Map.Navigator.ZoomTo(2); // Zoom nivel mundial
        }

        // Centrar el mapa en una ubicación específica
        public void CentrarEnUbicacion(double latitud, double longitud, double zoom = 10)
        {
            var (x, y) = SphericalMercator.FromLonLat(longitud, latitud);
            var punto = new MPoint(x, y);
            mapControl.Map.Navigator.CenterOn(punto);
            mapControl.Map.Navigator.ZoomTo(zoom);
        }

        // Calcular distancia entre dos puntos usando la fórmula de Haversine
        public static double CalcularDistanciaHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double RadioTierraKm = 6371.0;
            
            double dLat = GradosARadianes(lat2 - lat1);
            double dLon = GradosARadianes(lon2 - lon1);
            
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(GradosARadianes(lat1)) * Math.Cos(GradosARadianes(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distancia = RadioTierraKm * c;
            
            return distancia;
        }

        private static double GradosARadianes(double grados)
        {
            return grados * Math.PI / 180.0;
        }

        // Agregar marcadores de familiares al mapa
        public void MostrarFamiliaresEnMapa(GrafoGeografico grafo)
        {
            // Remover capa anterior si existe
            var capasExistentes = mapControl.Map.Layers.FindLayer("Familiares").ToList();
            foreach (var capa in capasExistentes)
            {
                mapControl.Map.Layers.Remove(capa);
            }

            // Crear nueva capa de marcadores
            var capaFamiliares = new WritableLayer
            {
                Name = "Familiares",
                Style = null
            };

            var nodos = grafo.ObtenerTodosNodos();
            for (int i = 0; i < nodos.Largo(); i++)
            {
                var nodo = nodos.Obtener(i);
                AgregarMarcador(capaFamiliares, nodo);
            }

            mapControl.Map.Layers.Add(capaFamiliares);
            mapControl.Refresh();
        }

        // Agregar un marcador individual al mapa
        private void AgregarMarcador(WritableLayer capa, GrafoGeografico.NodoGrafo nodo)
        {
            // Convertir coordenadas geográficas a Web Mercator
            var (x, y) = SphericalMercator.FromLonLat(nodo.Longitud, nodo.Latitud);
            var punto = new Point(x, y);

            // Crear feature con geometría de punto
            var feature = new GeometryFeature
            {
                Geometry = punto
            };

            // Guardar datos del nodo en el feature para eventos de click
            feature["Cedula"] = nodo.Cedula;
            feature["Nombre"] = nodo.Nombre;
            feature["FotoRuta"] = nodo.FotoRuta;

            // Estilo del marcador
            feature.Styles.Add(new SymbolStyle
            {
                SymbolScale = 0.8,
                Fill = new Brush(Color.FromArgb(200, 33, 150, 243)), // Azul semi-transparente
                Outline = new Pen(Color.White, 2)
            });

            capa.Add(feature);
        }

        // Limpiar todos los marcadores del mapa
        public void LimpiarMarcadores()
        {
            var capasExistentes = mapControl.Map.Layers.FindLayer("Familiares").ToList();
            foreach (var capa in capasExistentes)
            {
                mapControl.Map.Layers.Remove(capa);
            }
            mapControl.Refresh();
        }
    }
}
