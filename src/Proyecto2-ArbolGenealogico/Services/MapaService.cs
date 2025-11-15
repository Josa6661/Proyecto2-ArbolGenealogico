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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Proyecto2_ArbolGenealogico.Services
{
    public class MapaService
    {
        private readonly MapControl mapControl;
        private Canvas overlayCanvas;
        private GrafoGeografico grafoActual;
        private GrafoGeografico.NodoGrafo nodoSeleccionado;

        public MapaService(MapControl mapControl)
        {
            this.mapControl = mapControl;
        }

        public void ConfigurarOverlayCanvas(Canvas canvas)
        {
            overlayCanvas = canvas;
            
            // Suscribirse a cambios en el viewport para actualizar posiciones
            if (mapControl.Map != null)
            {
                mapControl.Map.Navigator.ViewportChanged += (s, e) =>
                {
                    
                    overlayCanvas.Dispatcher.InvokeAsync(() => ActualizarPosicionesCanvas(), DispatcherPriority.Normal);
                };
            }
        }

        // Configuración inicial del mapa con OpenStreetMap
        public void ConfigurarMapa()
        {
            // Configurar el mapa con OpenStreetMap
            mapControl.Map = new Map
            {
                CRS = "EPSG:3857",
                BackColor = Mapsui.Styles.Color.FromString("#0B1524") // Color de fondo igual al tema
            };

            // Agregar capa de mapa base de OpenStreetMap
            var tileLayer = OpenStreetMap.CreateTileLayer();
            mapControl.Map.Layers.Add(tileLayer);

            // Centrar el mapa en una posición inicial
            var centerPoint = new MPoint(0,0);
            mapControl.Map.Navigator.CenterOn(centerPoint);
            mapControl.Map.Navigator.ZoomTo(2); // Zoom nivel mundial
            
            // Evento para limpiar líneas al hacer clic en el mapa
            mapControl.MouseLeftButtonDown += (s, e) =>
            {
                // Verificar que no se hizo clic en el Canvas overlay
                if (overlayCanvas != null && !overlayCanvas.IsMouseOver)
                {
                    LimpiarLineasDistancia();
                }
            };
            
            // Suscribirse a cambios en el viewport
            mapControl.Map.Navigator.ViewportChanged += (s, e) =>
            {
                // ViewportChanged puede dispararse desde otro hilo, usar Dispatcher
                if (overlayCanvas != null)
                {
                    overlayCanvas.Dispatcher.InvokeAsync(() => ActualizarPosicionesCanvas(), DispatcherPriority.Normal);
                }
            };
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

        // Mostrar distancias visualmente en el mapa con líneas y etiquetas
        private void MostrarDistancias(GrafoGeografico.NodoGrafo nodoSeleccionado)
        {
            if (grafoActual == null || overlayCanvas == null)
                return;

            // Guardar el nodo seleccionado para actualizar posiciones al mover el mapa
            this.nodoSeleccionado = nodoSeleccionado;

            // Limpiar líneas anteriores
            LimpiarLineasDistancia();

            var distancias = grafoActual.ObtenerDistanciasDesde(nodoSeleccionado.Cedula);
            
            if (distancias.Largo() == 0)
            {
                MessageBox.Show($"{nodoSeleccionado.Nombre} no tiene otros familiares con ubicación registrada.", 
                    "Sin Conexiones", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var todosNodos = grafoActual.ObtenerTodosNodos();

            // Dibujar líneas desde el nodo seleccionado a cada otro nodo
            for (int i = 0; i < distancias.Largo(); i++)
            {
                var distancia = distancias.Obtener(i);
                
                // Buscar el nodo destino
                GrafoGeografico.NodoGrafo nodoDestino = null;
                for (int j = 0; j < todosNodos.Largo(); j++)
                {
                    if (todosNodos.Obtener(j).Cedula == distancia.cedula)
                    {
                        nodoDestino = todosNodos.Obtener(j);
                        break;
                    }
                }

                if (nodoDestino != null)
                {
                    DibujarLineaDistancia(nodoSeleccionado, nodoDestino, distancia.distancia);
                }
            }

            // Resaltar el nodo seleccionado
            ResaltarNodoSeleccionado(nodoSeleccionado);
        }

        // Limpiar todas las líneas de distancia dibujadas anteriormente
        private void LimpiarLineasDistancia()
        {
            if (overlayCanvas == null)
                return;

            var elementosAEliminar = new System.Collections.Generic.List<UIElement>();
            foreach (var child in overlayCanvas.Children)
            {
                // Eliminar líneas (Tag es tupla de nodos)
                if (child is Line linea)
                {
                    elementosAEliminar.Add(linea);
                }
                // Eliminar etiquetas de distancia 
                else if (child is TextBlock textBlock && textBlock.Text != null && textBlock.Text.EndsWith(" km"))
                {
                    elementosAEliminar.Add(textBlock);
                }
                // Eliminar círculo de resaltado (Tag es un nodo y es Ellipse)
                else if (child is Ellipse ellipse && ellipse.Tag is GrafoGeografico.NodoGrafo)
                {
                    elementosAEliminar.Add(ellipse);
                }
            }

            foreach (var elemento in elementosAEliminar)
            {
                overlayCanvas.Children.Remove(elemento);
            }

            // Limpiar el nodo seleccionado
            nodoSeleccionado = null;
        }

        // Dibujar una línea entre dos nodos con la distancia etiquetada
        private void DibujarLineaDistancia(GrafoGeografico.NodoGrafo origen, GrafoGeografico.NodoGrafo destino, double distanciaKm)
        {
            if (overlayCanvas == null || mapControl.Map?.Navigator?.Viewport == null)
                return;

            var viewport = mapControl.Map.Navigator.Viewport;
            if (double.IsInfinity(viewport.Width) || double.IsInfinity(viewport.Height) || 
                viewport.Width == 0 || viewport.Height == 0)
                return;

            // Convertir coordenadas geográficas a pantalla
            var (xOrigen, yOrigen) = SphericalMercator.FromLonLat(origen.Longitud, origen.Latitud);
            var (xDestino, yDestino) = SphericalMercator.FromLonLat(destino.Longitud, destino.Latitud);

            var worldOrigen = new MPoint(xOrigen, yOrigen);
            var worldDestino = new MPoint(xDestino, yDestino);

            var screenXOrigen = (worldOrigen.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
            var screenYOrigen = (viewport.CenterY - worldOrigen.Y) / viewport.Resolution + viewport.Height / 2.0;
            var screenXDestino = (worldDestino.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
            var screenYDestino = (viewport.CenterY - worldDestino.Y) / viewport.Resolution + viewport.Height / 2.0;

            // Validar coordenadas
            if (double.IsInfinity(screenXOrigen) || double.IsNaN(screenXOrigen) ||
                double.IsInfinity(screenYOrigen) || double.IsNaN(screenYOrigen) ||
                double.IsInfinity(screenXDestino) || double.IsNaN(screenXDestino) ||
                double.IsInfinity(screenYDestino) || double.IsNaN(screenYDestino))
                return;

            // Crear la línea
            var linea = new System.Windows.Shapes.Line
            {
                X1 = screenXOrigen,
                Y1 = screenYOrigen,
                X2 = screenXDestino,
                Y2 = screenYDestino,
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 255, 152, 0)), // Naranja semi-transparente
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 5, 3 }, // Línea punteada
                Tag = (origen, destino) // Guardar ambos nodos directamente
            };

            overlayCanvas.Children.Add(linea);

            // Crear etiqueta con la distancia en el punto medio
            var puntoMedioX = (screenXOrigen + screenXDestino) / 2.0;
            var puntoMedioY = (screenYOrigen + screenYDestino) / 2.0;

            var etiquetaDistancia = new TextBlock
            {
                Text = $"{distanciaKm:F0} km",
                Foreground = Brushes.White,
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(220, 255, 152, 0)), // Naranja
                Padding = new Thickness(6, 3, 6, 3),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Tag = (origen, destino) // Guardar ambos nodos directamente
            };

            Canvas.SetLeft(etiquetaDistancia, puntoMedioX - 30);
            Canvas.SetTop(etiquetaDistancia, puntoMedioY - 12);

            overlayCanvas.Children.Add(etiquetaDistancia);
        }

        // Resaltar el nodo seleccionado con un borde brillante
        private void ResaltarNodoSeleccionado(GrafoGeografico.NodoGrafo nodo)
        {
            if (overlayCanvas == null || mapControl.Map?.Navigator?.Viewport == null)
                return;

            var viewport = mapControl.Map.Navigator.Viewport;
            if (double.IsInfinity(viewport.Width) || double.IsInfinity(viewport.Height) || 
                viewport.Width == 0 || viewport.Height == 0)
                return;

            var (x, y) = SphericalMercator.FromLonLat(nodo.Longitud, nodo.Latitud);
            var worldPoint = new MPoint(x, y);

            var screenX = (worldPoint.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
            var screenY = (viewport.CenterY - worldPoint.Y) / viewport.Resolution + viewport.Height / 2.0;

            if (double.IsInfinity(screenX) || double.IsNaN(screenX) ||
                double.IsInfinity(screenY) || double.IsNaN(screenY))
                return;

            // Crear círculo de resaltado
            var circuloResaltado = new System.Windows.Shapes.Ellipse
            {
                Width = 70,
                Height = 70,
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 152, 0)), // Naranja brillante
                StrokeThickness = 4,
                Fill = Brushes.Transparent,
                Tag = nodo // Guardar el nodo directamente
            };

            Canvas.SetLeft(circuloResaltado, screenX - 35);
            Canvas.SetTop(circuloResaltado, screenY - 35);

            overlayCanvas.Children.Add(circuloResaltado);
        }

        // Agregar marcadores de familiares al mapa usando Canvas Overlay
        public void MostrarFamiliaresEnMapa(GrafoGeografico grafo)
        {
            if (overlayCanvas == null)
                return;

            // Guardar referencia al grafo actual para mostrar distancias
            grafoActual = grafo;

            // Limpiar marcadores y canvas overlay
            LimpiarMarcadores();
            overlayCanvas.Children.Clear();

            // Verificar que el viewport esté inicializado
            if (mapControl.Map?.Navigator?.Viewport == null || 
                double.IsInfinity(mapControl.Map.Navigator.Viewport.Width) ||
                double.IsInfinity(mapControl.Map.Navigator.Viewport.Height) ||
                mapControl.Map.Navigator.Viewport.Width == 0 ||
                mapControl.Map.Navigator.Viewport.Height == 0)
            {
                // Esperar a que el viewport se inicialice
                mapControl.Loaded += (s, e) => 
                {
                    // Solo agregar si este grafo sigue siendo el actual
                    if (grafoActual == grafo)
                    {
                        var nodos = grafo.ObtenerTodosNodos();
                        for (int i = 0; i < nodos.Largo(); i++)
                        {
                            var nodo = nodos.Obtener(i);
                            AgregarMarcadorEnCanvas(nodo);
                        }
                    }
                };
                return;
            }

            var nodos = grafo.ObtenerTodosNodos();
            for (int i = 0; i < nodos.Largo(); i++)
            {
                var nodo = nodos.Obtener(i);
                AgregarMarcadorEnCanvas(nodo);
            }
        }

        // Agregar marcador de foto en el Canvas overlay
        private void AgregarMarcadorEnCanvas(GrafoGeografico.NodoGrafo nodo)
        {
            if (overlayCanvas == null)
                return;

            // Crear contenedor para la foto
            var fotoContainer = new Border
            {
                Width = 50,
                Height = 50,
                CornerRadius = new CornerRadius(25),
                BorderBrush = Brushes.White,
                BorderThickness = new Thickness(3),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = nodo, // Guardar nodo para eventos
                IsHitTestVisible = true // Solo las fotos son clickeables
            };

            // Intentar cargar la foto
            if (!string.IsNullOrEmpty(nodo.FotoRuta) && File.Exists(nodo.FotoRuta))
            {
                try
                {
                    var imageBrush = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(nodo.FotoRuta, UriKind.Absolute)),
                        Stretch = Stretch.UniformToFill
                    };
                    fotoContainer.Background = imageBrush;
                }
                catch
                {
                    // Si falla cargar la imagen, usar color por defecto
                    fotoContainer.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 150, 243));
                }
            }
            else
            {
                // Sin foto, usar color azul por defecto
                fotoContainer.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 150, 243));
            }

            // Agregar etiqueta con el nombre
            var nombreLabel = new TextBlock
            {
                Text = nodo.Nombre,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 0, 0, 0)),
                Padding = new Thickness(5, 2, 5, 2),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Tag = nodo,
                IsHitTestVisible = true // Labels también son clickeables
            };

            // Convertir coordenadas y posicionar
            var (x, y) = SphericalMercator.FromLonLat(nodo.Longitud, nodo.Latitud);
            var viewport = mapControl.Map.Navigator.Viewport;
            
            // Validar que el viewport tenga valores válidos
            if (double.IsInfinity(viewport.Resolution) || 
                double.IsInfinity(viewport.Width) || double.IsInfinity(viewport.Height) ||
                viewport.Resolution == 0 || viewport.Width == 0 || viewport.Height == 0)
            {
                return; // No podemos posicionar sin viewport válido
            }
            
            var worldPoint = new MPoint(x, y);
            
            // Convertir de coordenadas del mundo a coordenadas de pantalla
            var screenX = (worldPoint.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
            var screenY = (viewport.CenterY - worldPoint.Y) / viewport.Resolution + viewport.Height / 2.0;
            
            // Validar coordenadas de pantalla antes de posicionar
            if (double.IsInfinity(screenX) || double.IsNaN(screenX) ||
                double.IsInfinity(screenY) || double.IsNaN(screenY))
            {
                return; // Coordenadas inválidas
            }

            Canvas.SetLeft(fotoContainer, screenX - 25);
            Canvas.SetTop(fotoContainer, screenY - 25);
            Canvas.SetLeft(nombreLabel, screenX - 40);
            Canvas.SetTop(nombreLabel, screenY + 30);

            // Agregar al canvas
            overlayCanvas.Children.Add(fotoContainer);
            overlayCanvas.Children.Add(nombreLabel);

            // Evento click para mostrar distancias
            fotoContainer.MouseLeftButtonDown += (s, e) =>
            {
                MostrarDistancias(nodo);
                e.Handled = true; // Marcar el evento como manejado
            };

            // También agregar evento al label
            nombreLabel.MouseLeftButtonDown += (s, e) =>
            {
                MostrarDistancias(nodo);
                e.Handled = true;
            };
        }

        // Actualizar posiciones de marcadores cuando se navega el mapa
        private void ActualizarPosicionesCanvas()
        {
            if (overlayCanvas == null)
                return;

            // Si no hay grafo actual, asegurar que el canvas esté vacío
            if (grafoActual == null)
            {
                if (overlayCanvas.Children.Count > 0)
                {
                    overlayCanvas.Children.Clear();
                }
                return;
            }

            // Validar que el viewport esté inicializado
            if (mapControl.Map?.Navigator?.Viewport == null)
                return;
                
            var viewport = mapControl.Map.Navigator.Viewport;
            if (double.IsInfinity(viewport.Width) ||
                double.IsInfinity(viewport.Height) || viewport.Width == 0 || viewport.Height == 0)
            {
                return;
            }

            // Recalcular posiciones de todos los elementos en el canvas
            foreach (var child in overlayCanvas.Children)
            {
                if (child is Border border && border.Tag is GrafoGeografico.NodoGrafo nodo)
                {
                    var (x, y) = SphericalMercator.FromLonLat(nodo.Longitud, nodo.Latitud);
                    var worldPoint = new MPoint(x, y);
                    
                    var screenX = (worldPoint.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
                    var screenY = (viewport.CenterY - worldPoint.Y) / viewport.Resolution + viewport.Height / 2.0;
                    
                    if (!double.IsInfinity(screenX) && !double.IsNaN(screenX) &&
                        !double.IsInfinity(screenY) && !double.IsNaN(screenY))
                    {
                        Canvas.SetLeft(border, screenX - 25);
                        Canvas.SetTop(border, screenY - 25);
                    }
                }
                else if (child is TextBlock label && label.Tag is GrafoGeografico.NodoGrafo labelNodo)
                {
                    var (x, y) = SphericalMercator.FromLonLat(labelNodo.Longitud, labelNodo.Latitud);
                    var worldPoint = new MPoint(x, y);
                    
                    var screenX = (worldPoint.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
                    var screenY = (viewport.CenterY - worldPoint.Y) / viewport.Resolution + viewport.Height / 2.0;
                    
                    if (!double.IsInfinity(screenX) && !double.IsNaN(screenX) &&
                        !double.IsInfinity(screenY) && !double.IsNaN(screenY))
                    {
                        Canvas.SetLeft(label, screenX - 40);
                        Canvas.SetTop(label, screenY + 30);
                    }
                }
                else if (child is Line linea && linea.Tag is (GrafoGeografico.NodoGrafo origen, GrafoGeografico.NodoGrafo destino))
                {
                    // Actualizar línea igual que las fotos
                    var (xOrigen, yOrigen) = SphericalMercator.FromLonLat(origen.Longitud, origen.Latitud);
                    var worldOrigen = new MPoint(xOrigen, yOrigen);
                    var screenXOrigen = (worldOrigen.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
                    var screenYOrigen = (viewport.CenterY - worldOrigen.Y) / viewport.Resolution + viewport.Height / 2.0;

                    var (xDestino, yDestino) = SphericalMercator.FromLonLat(destino.Longitud, destino.Latitud);
                    var worldDestino = new MPoint(xDestino, yDestino);
                    var screenXDestino = (worldDestino.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
                    var screenYDestino = (viewport.CenterY - worldDestino.Y) / viewport.Resolution + viewport.Height / 2.0;

                    if (!double.IsInfinity(screenXOrigen) && !double.IsNaN(screenXOrigen) &&
                        !double.IsInfinity(screenYOrigen) && !double.IsNaN(screenYOrigen) &&
                        !double.IsInfinity(screenXDestino) && !double.IsNaN(screenXDestino) &&
                        !double.IsInfinity(screenYDestino) && !double.IsNaN(screenYDestino))
                    {
                        linea.X1 = screenXOrigen;
                        linea.Y1 = screenYOrigen;
                        linea.X2 = screenXDestino;
                        linea.Y2 = screenYDestino;
                    }
                }
                else if (child is TextBlock etiquetaDist && etiquetaDist.Tag is (GrafoGeografico.NodoGrafo origenEtiq, GrafoGeografico.NodoGrafo destinoEtiq) && etiquetaDist.Text.EndsWith(" km"))
                {
                    // Actualizar etiqueta igual que las fotos
                    var (xOrigen, yOrigen) = SphericalMercator.FromLonLat(origenEtiq.Longitud, origenEtiq.Latitud);
                    var worldOrigen = new MPoint(xOrigen, yOrigen);
                    var screenXOrigen = (worldOrigen.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
                    var screenYOrigen = (viewport.CenterY - worldOrigen.Y) / viewport.Resolution + viewport.Height / 2.0;

                    var (xDestino, yDestino) = SphericalMercator.FromLonLat(destinoEtiq.Longitud, destinoEtiq.Latitud);
                    var worldDestino = new MPoint(xDestino, yDestino);
                    var screenXDestino = (worldDestino.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
                    var screenYDestino = (viewport.CenterY - worldDestino.Y) / viewport.Resolution + viewport.Height / 2.0;

                    if (!double.IsInfinity(screenXOrigen) && !double.IsNaN(screenXOrigen) &&
                        !double.IsInfinity(screenYOrigen) && !double.IsNaN(screenYOrigen) &&
                        !double.IsInfinity(screenXDestino) && !double.IsNaN(screenXDestino) &&
                        !double.IsInfinity(screenYDestino) && !double.IsNaN(screenYDestino))
                    {
                        var puntoMedioX = (screenXOrigen + screenXDestino) / 2.0;
                        var puntoMedioY = (screenYOrigen + screenYDestino) / 2.0;
                        Canvas.SetLeft(etiquetaDist, puntoMedioX - 30);
                        Canvas.SetTop(etiquetaDist, puntoMedioY - 12);
                    }
                }
                else if (child is Ellipse circulo && circulo.Tag is GrafoGeografico.NodoGrafo nodoCirculo)
                {
                    // Actualizar círculo igual que las fotos
                    var (x, y) = SphericalMercator.FromLonLat(nodoCirculo.Longitud, nodoCirculo.Latitud);
                    var worldPoint = new MPoint(x, y);
                    var screenX = (worldPoint.X - viewport.CenterX) / viewport.Resolution + viewport.Width / 2.0;
                    var screenY = (viewport.CenterY - worldPoint.Y) / viewport.Resolution + viewport.Height / 2.0;

                    if (!double.IsInfinity(screenX) && !double.IsNaN(screenX) &&
                        !double.IsInfinity(screenY) && !double.IsNaN(screenY))
                    {
                        Canvas.SetLeft(circulo, screenX - 35);
                        Canvas.SetTop(circulo, screenY - 35);
                    }
                }
            }
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

        // Limpiar completamente el mapa y el overlay
        public void LimpiarMapaCompleto()
        {
            // Primero poner en null para detener actualizaciones
            grafoActual = null;
            nodoSeleccionado = null;
            
            // Luego limpiar marcadores y canvas
            LimpiarMarcadores();
            
            if (overlayCanvas != null)
            {
                overlayCanvas.Children.Clear();
            }
        }
    }
}
