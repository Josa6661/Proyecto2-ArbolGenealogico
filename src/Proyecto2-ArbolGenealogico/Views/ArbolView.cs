using Proyecto2_ArbolGenealogico.DataStructures;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Proyecto2_ArbolGenealogico.Views
{
    public class ArbolView
    {
        private readonly Canvas canvas;
        private readonly double nodoAncho;
        private readonly double nodoAlto;
        private readonly double espacioHorizontal;
        private readonly double espacioVertical;

        public ArbolView(Canvas canvas, double nodoAncho = 120, double nodoAlto = 100, 
            double espacioHorizontal = 80, double espacioVertical = 140)
        {
            this.canvas = canvas;
            this.nodoAncho = nodoAncho;
            this.nodoAlto = nodoAlto;
            this.espacioHorizontal = espacioHorizontal;
            this.espacioVertical = espacioVertical;
        }

        // Dibuja el árbol completo en el Canvas
        public void DibujarArbol(ArbolGenealogico arbol)
        {
            canvas.Children.Clear();

            if (!arbol.TieneRaiz())
                return;

            // Calcular el ancho total del árbol para centrar y dimensionar el Canvas
            double anchoArbol = CalcularAncho(arbol.Raiz);
            double alturaArbol = CalcularAltura(arbol.Raiz) * espacioVertical + nodoAlto + 100;
            
            // Asegurar un tamaño mínimo del Canvas
            double anchoCanvas = Math.Max(anchoArbol + 400, 1200);
            double alturaCanvas = Math.Max(alturaArbol, 800);
            
            canvas.Width = anchoCanvas;
            canvas.Height = alturaCanvas;

            // Centrar el árbol en el Canvas
            double startX = anchoCanvas / 2;
            double startY = 60;
            DibujarNodo(arbol.Raiz, startX, startY);
        }

        // Calcula la altura del árbol (número de niveles)
        private int CalcularAltura(NodoFamiliar nodo)
        {
            if (nodo == null || nodo.Hijos.Largo() == 0)
                return 1;

            int alturaMaxima = 0;
            for (int i = 0; i < nodo.Hijos.Largo(); i++)
            {
                int alturaHijo = CalcularAltura(nodo.Hijos.Obtener(i));
                if (alturaHijo > alturaMaxima)
                    alturaMaxima = alturaHijo;
            }
            return alturaMaxima + 1;
        }

        // Dibuja un nodo y sus hijos recursivamente
        private double DibujarNodo(NodoFamiliar persona, double x, double y)
        {
            double anchoTotal = nodoAncho;
            double xNodoPrincipal = x;

            // Si tiene cónyuge, ajustar posiciones para dibujar ambos lado a lado
            if (persona.Conyuge != null)
            {
                double espacioEntreConyuges = 20;
                anchoTotal = nodoAncho * 2 + espacioEntreConyuges;
                xNodoPrincipal = x - (nodoAncho + espacioEntreConyuges) / 2;
                double xConyuge = xNodoPrincipal + nodoAncho + espacioEntreConyuges;

                // Dibuja el nodo del cónyuge
                Ellipse nodoConyuge = new Ellipse
                {
                    Width = nodoAncho,
                    Height = nodoAlto,
                    Stroke = new SolidColorBrush(Color.FromRgb(129, 199, 132)), // Verde para cónyuge
                    StrokeThickness = 2
                };
                TextBlock textoConyuge = new TextBlock
                {
                    Text = persona.Conyuge.Nombre,
                    Foreground = Brushes.White,
                    FontSize = 11,
                    TextAlignment = TextAlignment.Center,
                    Width = nodoAncho - 10,
                    TextWrapping = TextWrapping.Wrap,
                    Padding = new Thickness(5)
                };

                // Medir el texto para centrarlo verticalmente
                textoConyuge.Measure(new Size(nodoAncho - 10, nodoAlto - 10));
                double alturaTextoConyuge = textoConyuge.DesiredSize.Height;

                Canvas.SetLeft(nodoConyuge, xConyuge - nodoAncho / 2);
                Canvas.SetTop(nodoConyuge, y);
                Canvas.SetLeft(textoConyuge, xConyuge - nodoAncho / 2 + 5);
                Canvas.SetTop(textoConyuge, y + (nodoAlto - alturaTextoConyuge) / 2);

                canvas.Children.Add(nodoConyuge);
                canvas.Children.Add(textoConyuge);

                // Línea horizontal conectando los cónyuges
                Line lineaConyuge = new Line
                {
                    X1 = xNodoPrincipal + nodoAncho / 2,
                    Y1 = y + nodoAlto / 2,
                    X2 = xConyuge - nodoAncho / 2,
                    Y2 = y + nodoAlto / 2,
                    Stroke = new SolidColorBrush(Color.FromRgb(129, 199, 132)),
                    StrokeThickness = 3
                };
                canvas.Children.Add(lineaConyuge);
            }

            // Dibuja el nodo principal
            Ellipse nodo = new Ellipse
            {
                Width = nodoAncho,
                Height = nodoAlto,
                Stroke = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                StrokeThickness = 2
            };
            TextBlock texto = new TextBlock
            {
                Text = persona.Nombre,
                Foreground = Brushes.White,
                FontSize = 11,
                TextAlignment = TextAlignment.Center,
                Width = nodoAncho - 10,
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(5)
            };

            // Medir el texto para centrarlo verticalmente
            texto.Measure(new Size(nodoAncho - 10, nodoAlto - 10));
            double alturaTexto = texto.DesiredSize.Height;

            Canvas.SetLeft(nodo, xNodoPrincipal - nodoAncho / 2);
            Canvas.SetTop(nodo, y);
            Canvas.SetLeft(texto, xNodoPrincipal - nodoAncho / 2 + 5);
            Canvas.SetTop(texto, y + (nodoAlto - alturaTexto) / 2);

            canvas.Children.Add(nodo);
            canvas.Children.Add(texto);

            if (persona.Hijos.Largo() == 0)
                return Math.Max(nodoAncho, anchoTotal);

            // Calcular ancho total de los hijos
            double totalAnchoHijos = 0;
            for (int i = 0; i < persona.Hijos.Largo(); i++)
                totalAnchoHijos += CalcularAncho(persona.Hijos.Obtener(i));

            double xInicio = x - totalAnchoHijos / 2;
            
            // Punto central de la pareja (si hay cónyuge)
            double xCentroPareja = x;

            for (int i = 0; i < persona.Hijos.Largo(); i++)
            {
                var hijo = persona.Hijos.Obtener(i);
                double anchoHijo = CalcularAncho(hijo);
                double xHijo = xInicio + anchoHijo / 2;

                // Línea de conexión (desde borde inferior del padre hasta borde superior del hijo)
                Line linea = new Line
                {
                    X1 = xCentroPareja,
                    Y1 = y + nodoAlto,  // Borde inferior del padre
                    X2 = xHijo,
                    Y2 = y + espacioVertical,  // Borde superior del hijo
                    Stroke = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                    StrokeThickness = 2
                };
                canvas.Children.Add(linea);

                // Dibuja hijo recursivamente
                DibujarNodo(hijo, xHijo, y + espacioVertical);
                xInicio += anchoHijo;
            }

            return Math.Max(totalAnchoHijos, anchoTotal);
        }

        // Calcula el ancho necesario para un nodo y sus descendientes
        private double CalcularAncho(NodoFamiliar persona)
        {
            // Ancho base: si tiene cónyuge, necesita espacio para ambos
            double anchoBase = nodoAncho + espacioHorizontal;
            if (persona.Conyuge != null)
            {
                anchoBase = (nodoAncho * 2 + 20) + espacioHorizontal; // Dos nodos + espacio entre ellos
            }

            if (persona.Hijos.Largo() == 0)
                return anchoBase;

            double ancho = 0;
            for (int i = 0; i < persona.Hijos.Largo(); i++)
                ancho += CalcularAncho(persona.Hijos.Obtener(i));

            return Math.Max(ancho, anchoBase);
        }
    }
}
