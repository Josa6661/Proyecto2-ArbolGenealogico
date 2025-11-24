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
        public Action<NodoFamiliar> OnNodoClick;

        public ArbolView(Canvas canvas, double nodoAncho = 120, double nodoAlto = 100, 
            double espacioHorizontal = 150, double espacioVertical = 160)
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

            // Calcular altura hacia arriba (ancestros) y hacia abajo (descendientes)
            int alturaAncestros = CalcularAlturaHaciaPadres(arbol.Raiz);
            int alturaDescendientes = CalcularAltura(arbol.Raiz);
            
            // Calcular el ancho total del árbol
            double anchoArbol = CalcularAnchoTotal(arbol.Raiz);
            
            // Calcular altura total del canvas
            double alturaTotal = (alturaAncestros + alturaDescendientes) * espacioVertical + nodoAlto + 200;
            
            // Asegurar un tamaño mínimo del Canvas
            double anchoCanvas = Math.Max(anchoArbol + 400, 1400);
            double alturaCanvas = Math.Max(alturaTotal, 1000);
            
            canvas.Width = anchoCanvas;
            canvas.Height = alturaCanvas;

            // Posicionar la raíz considerando los ancestros arriba
            double startX = anchoCanvas / 2;
            double startY = alturaAncestros * espacioVertical + 100;
            
            DibujarNodoCompleto(arbol.Raiz, startX, startY);
        }

        // Calcula la altura hacia los padres/ancestros
        private int CalcularAlturaHaciaPadres(NodoFamiliar nodo)
        {
            if (nodo == null || nodo.Padres.Largo() == 0)
                return 0;

            int alturaMaxima = 0;
            for (int i = 0; i < nodo.Padres.Largo(); i++)
            {
                int alturaPadre = CalcularAlturaHaciaPadres(nodo.Padres.Obtener(i));
                if (alturaPadre > alturaMaxima)
                    alturaMaxima = alturaPadre;
            }
            return alturaMaxima + 1;
        }

        // Calcula el ancho necesario para el subárbol de ancestros (padres, abuelos, etc.)
        private double CalcularAnchoAncestros(NodoFamiliar nodo)
        {
            if (nodo == null || nodo.Padres.Largo() == 0)
                return nodoAncho;

            if (nodo.Padres.Largo() == 1)
            {
                // Un solo padre - recursivo
                return CalcularAnchoAncestros(nodo.Padres.Obtener(0));
            }
            else if (nodo.Padres.Largo() == 2)
            {
                // Dos padres - calcular ancho de cada uno y sumar
                var padre1 = nodo.Padres.Obtener(0);
                var padre2 = nodo.Padres.Obtener(1);
                
                double anchoPadre1 = CalcularAnchoAncestros(padre1);
                double anchoPadre2 = CalcularAnchoAncestros(padre2);
                
                // El ancho total es la suma de ambos ancestros más el espacio entre ellos
                return anchoPadre1 + anchoPadre2 + espacioHorizontal;
            }
            
            return nodoAncho;
        }

        // Calcula el ancho total considerando padres y hermanos
        private double CalcularAnchoTotal(NodoFamiliar nodo)
        {
            if (nodo == null)
                return nodoAncho;

            // Calcular ancho del nodo actual (con cónyuge si lo tiene)
            double anchoNodo = nodoAncho + espacioHorizontal;
            if (nodo.Conyuge != null)
                anchoNodo = (nodoAncho * 2 + 100) + espacioHorizontal; // Actualizado a 100

            // Calcular ancho de los hijos
            double anchoHijos = 0;
            for (int i = 0; i < nodo.Hijos.Largo(); i++)
                anchoHijos += CalcularAncho(nodo.Hijos.Obtener(i));

            // Calcular ancho de los padres del nodo principal
            double anchoPadresNodo = 0;
            if (nodo.Padres.Largo() > 0)
            {
                for (int i = 0; i < nodo.Padres.Largo(); i++)
                {
                    var padre = nodo.Padres.Obtener(i);
                    anchoPadresNodo += CalcularAncho(padre);
                }
            }
            
            // Calcular ancho de los padres del cónyuge
            double anchoPadresConyuge = 0;
            if (nodo.Conyuge != null && nodo.Conyuge.Padres.Largo() > 0)
            {
                for (int i = 0; i < nodo.Conyuge.Padres.Largo(); i++)
                {
                    var padre = nodo.Conyuge.Padres.Obtener(i);
                    anchoPadresConyuge += CalcularAncho(padre);
                }
            }
            
            // Sumar los anchos de padres de ambos
            double anchoPadresTotal = anchoPadresNodo + anchoPadresConyuge;

            return Math.Max(Math.Max(anchoNodo, anchoHijos), anchoPadresTotal);
        }

        // Dibuja un nodo completo con sus padres (arriba) y sus hijos (abajo)
        private void DibujarNodoCompleto(NodoFamiliar persona, double x, double y)
        {
            double xPersonaReal = x;
            double xConyugeReal = x;
            
            if (persona.Conyuge != null)
            {
                // Calcular ancho necesario para ancestros de cada cónyuge
                double anchoPersona = Math.Max(CalcularAnchoAncestros(persona), nodoAncho);
                double anchoConyuge = Math.Max(CalcularAnchoAncestros(persona.Conyuge), nodoAncho);
                
                // Espacio mínimo entre cónyuges
                double espacioMinimo = 120;
                
                // Calcular espacio dinámico basado en ancestros
                double espacioNecesario = Math.Max(espacioMinimo, (anchoPersona / 2) + espacioMinimo + (anchoConyuge / 2));
                
                double xPersona = x - espacioNecesario / 2;
                double xConyuge = x + espacioNecesario / 2;

                // Dibujar los padres de la persona principal y obtener su posición real
                if (persona.Padres.Largo() > 0)
                {
                    xPersonaReal = DibujarPadres(persona, xPersona, y);
                }
                else
                {
                    xPersonaReal = xPersona;
                }

                // Dibujar los padres del cónyuge y obtener su posición real
                if (persona.Conyuge.Padres.Largo() > 0)
                {
                    xConyugeReal = DibujarPadres(persona.Conyuge, xConyuge, y);
                }
                else
                {
                    xConyugeReal = xConyuge;
                }
                
                // Calcular el centro entre las posiciones reales de los cónyuges
                double xCentroReal = (xPersonaReal + xConyugeReal) / 2;
                
                // Dibujar el nodo actual y sus descendientes en la posición real
                DibujarNodoYDescendientes(persona, xCentroReal, y);
            }
            else
            {
                // Sin cónyuge, dibujar padres en la posición central y obtener posición real
                if (persona.Padres.Largo() > 0)
                {
                    xPersonaReal = DibujarPadres(persona, x, y);
                }
                else
                {
                    xPersonaReal = x;
                }
                
                // Dibujar el nodo actual y sus descendientes en la posición real
                DibujarNodoYDescendientes(persona, xPersonaReal, y);
            }
        }

        // Dibuja los padres de un nodo específico (conectados solo a ESE nodo)
        // Devuelve la posición X donde debe dibujarse el hijo para estar alineado con sus padres
        private double DibujarPadres(NodoFamiliar hijo, double xHijo, double yHijo)
        {
            if (hijo.Padres.Largo() == 0)
                return xHijo; // Sin padres, mantener posición original

            double yPadres = yHijo - espacioVertical;
            
            if (hijo.Padres.Largo() == 1)
            {
                // Un solo padre - conectar directamente al hijo
                var padre = hijo.Padres.Obtener(0);
                double xPadreReal = DibujarPadres(padre, xHijo, yPadres); // Recursivo hacia arriba
                DibujarNodoSimple(padre, xPadreReal, yPadres);
                
                // Línea de conexión directa del padre al hijo
                DibujarLineaConexion(xPadreReal, yPadres + nodoAlto, xPadreReal, yHijo, Color.FromRgb(100, 181, 246));
                
                return xPadreReal; // El hijo debe estar en la misma X que su único padre
            }
            else if (hijo.Padres.Largo() == 2)
            {
                // Dos padres (pareja) - calcular espacio dinámico basado en sus ancestros
                var padre1 = hijo.Padres.Obtener(0);
                var padre2 = hijo.Padres.Obtener(1);
                
                // Calcular ancho necesario para los ancestros de cada padre
                double anchoPadre1 = Math.Max(CalcularAnchoAncestros(padre1), nodoAncho);
                double anchoPadre2 = Math.Max(CalcularAnchoAncestros(padre2), nodoAncho);
                
                // Espacio mínimo entre los nodos de los padres
                double espacioMinimo = 120;
                
                // Calcular el espacio total necesario entre los centros de los padres
                // Debe ser suficiente para que sus ancestros no se sobrepongan
                double espacioNecesario = (anchoPadre1 / 2) + espacioMinimo + (anchoPadre2 / 2);
                
                // Posicionar los padres
                double xPadre1 = xHijo - espacioNecesario / 2;
                double xPadre2 = xHijo + espacioNecesario / 2;
                
                // Dibujar ancestros de los padres recursivamente (obtener sus posiciones reales)
                double xPadre1Real = DibujarPadres(padre1, xPadre1, yPadres);
                double xPadre2Real = DibujarPadres(padre2, xPadre2, yPadres);
                
                // Dibujar los padres en sus posiciones reales
                DibujarNodoSimple(padre1, xPadre1Real, yPadres);
                DibujarNodoSimple(padre2, xPadre2Real, yPadres);
                
                // Línea horizontal entre padres (conexión de pareja)
                DibujarLineaConexion(xPadre1Real + nodoAncho/2, yPadres + nodoAlto/2, 
                                    xPadre2Real - nodoAncho/2, yPadres + nodoAlto/2, 
                                    Color.FromRgb(129, 199, 132));
                
                // Punto medio entre los padres (donde debe estar el hijo)
                double xMedio = (xPadre1Real + xPadre2Real) / 2;
                
                // Línea vertical desde el punto medio hasta el hijo
                DibujarLineaConexion(xMedio, yPadres + nodoAlto, xMedio, yHijo, Color.FromRgb(100, 181, 246));
                
                // DIBUJAR TODOS LOS HERMANOS DEL HIJO (otros hijos de la pareja)
                // Se dibujan DESPUÉS de la línea del hijo de referencia
                // Determinar dirección: izquierda si es persona principal, derecha si es cónyuge
                bool dibujarIzquierda = hijo.Conyuge == null || xHijo <= canvas.Width / 2;
                DibujarHermanosConectados(padre1, hijo, xMedio, yHijo, xPadre1Real, xPadre2Real, yPadres, dibujarIzquierda);
                
                return xMedio; // El hijo debe estar centrado entre sus padres
            }
            
            return xHijo; // Por defecto, mantener posición
        }
        
        // Dibuja los hermanos de un nodo conectados a los padres
        private void DibujarHermanosConectados(NodoFamiliar padre, NodoFamiliar hijoReferencia, double xHijoRef, double yHijos, double xPadre1, double xPadre2, double yPadres, bool dibujarIzquierda)
        {
            if (padre == null || padre.Hijos.Largo() <= 1)
                return; // No hay hermanos que dibujar
            
            // Calcular punto medio entre los padres (donde salen las conexiones)
            double xMedioPadres = (xPadre1 + xPadre2) / 2;
            
            // Calcular ancho total de los hermanos (sin el hijo de referencia)
            double anchoTotalHermanos = 0;
            int cantidadHermanos = 0;
            for (int i = 0; i < padre.Hijos.Largo(); i++)
            {
                var hijo = padre.Hijos.Obtener(i);
                if (hijo != hijoReferencia)
                {
                    anchoTotalHermanos += CalcularAncho(hijo);
                    cantidadHermanos++;
                }
            }
            
            if (cantidadHermanos == 0)
                return;
            
            // Calcular el ancho que ocupa el hijo de referencia (con cónyuge si lo tiene)
            double anchoHijoRef = CalcularAncho(hijoReferencia);
            double espacioEntreHermanos = 20;
            
            if (dibujarIzquierda)
            {
                // HERMANOS A LA IZQUIERDA (caso de Madre)
                // Empezar desde la izquierda del hijo de referencia y retroceder
                double xActual = xHijoRef - (anchoHijoRef / 2) - espacioEntreHermanos;
                
                // Dibujar hermanos de derecha a izquierda (más cercanos primero)
                for (int i = padre.Hijos.Largo() - 1; i >= 0; i--)
                {
                    var hijo = padre.Hijos.Obtener(i);
                    
                    // Saltar el hijo de referencia
                    if (hijo == hijoReferencia)
                        continue;
                    
                    double anchoHermano = CalcularAncho(hijo);
                    double xHermano = xActual - (anchoHermano / 2);
                    
                    // Dibujar línea de conexión
                    DibujarLineaConexion(xMedioPadres, yPadres + nodoAlto, xHermano, yHijos, Color.FromRgb(100, 181, 246));
                    
                    // Dibujar el hermano y sus descendientes
                    DibujarNodoYDescendientes(hijo, xHermano, yHijos);
                    
                    // Mover a la izquierda para el siguiente hermano
                    xActual = xActual - anchoHermano - espacioEntreHermanos;
                }
            }
            else
            {
                // HERMANOS A LA DERECHA (caso de Padre/Cónyuge)
                // Empezar desde la derecha del hijo de referencia y avanzar
                double xActual = xHijoRef + (anchoHijoRef / 2) + espacioEntreHermanos;
                
                // Dibujar hermanos de izquierda a derecha
                for (int i = 0; i < padre.Hijos.Largo(); i++)
                {
                    var hijo = padre.Hijos.Obtener(i);
                    
                    // Saltar el hijo de referencia
                    if (hijo == hijoReferencia)
                        continue;
                    
                    double anchoHermano = CalcularAncho(hijo);
                    double xHermano = xActual + (anchoHermano / 2);
                    
                    // Dibujar línea de conexión
                    DibujarLineaConexion(xMedioPadres, yPadres + nodoAlto, xHermano, yHijos, Color.FromRgb(100, 181, 246));
                    
                    // Dibujar el hermano y sus descendientes
                    DibujarNodoYDescendientes(hijo, xHermano, yHijos);
                    
                    // Mover a la derecha para el siguiente hermano
                    xActual = xActual + anchoHermano + espacioEntreHermanos;
                }
            }
        }

        // Dibuja un nodo simple (sin recursión de descendientes)
        private void DibujarNodoSimple(NodoFamiliar persona, double x, double y)
        {
            Ellipse nodo = new Ellipse
            {
                Width = nodoAncho,
                Height = nodoAlto,
                Stroke = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(40, 100, 181, 246))
            };
            
            TextBlock texto = new TextBlock
            {
                Text = persona.Nombre,
                Foreground = Brushes.White,
                FontSize = 11,
                TextAlignment = TextAlignment.Center,
                Width = nodoAncho - 10,
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(5),
                Tag = persona,
                Cursor = System.Windows.Input.Cursors.Hand
            };
            texto.MouseLeftButtonDown += (s, e) => OnNodoClick?.Invoke(persona);

            texto.Measure(new Size(nodoAncho - 10, nodoAlto - 10));
            double alturaTexto = texto.DesiredSize.Height;

            Canvas.SetLeft(nodo, x - nodoAncho / 2);
            Canvas.SetTop(nodo, y);
            Canvas.SetLeft(texto, x - nodoAncho / 2 + 5);
            Canvas.SetTop(texto, y + (nodoAlto - alturaTexto) / 2);

            canvas.Children.Add(nodo);
            canvas.Children.Add(texto);
        }

        // Método auxiliar para dibujar líneas de conexión
        private void DibujarLineaConexion(double x1, double y1, double x2, double y2, Color color)
        {
            Line linea = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = new SolidColorBrush(color),
                StrokeThickness = 2
            };
            canvas.Children.Add(linea);
        }

        // Dibuja el nodo actual y todos sus descendientes (hijos, nietos, etc.)
        private double DibujarNodoYDescendientes(NodoFamiliar persona, double x, double y)
        {
            // IMPORTANTE: Solo dibujamos este nodo y sus descendientes
            // No dibujamos hermanos aquí porque eso se maneja a nivel superior
            
            double anchoTotal = nodoAncho;
            double xNodoPrincipal = x;

            // Si tiene cónyuge, ajustar posiciones para dibujar ambos lado a lado
            if (persona.Conyuge != null)
            {
                double espacioEntreConyuges = 100; // Aumentado de 20 a 100 para evitar sobreposición
                anchoTotal = nodoAncho * 2 + espacioEntreConyuges;
                xNodoPrincipal = x - (nodoAncho + espacioEntreConyuges) / 2;
                double xConyuge = xNodoPrincipal + nodoAncho + espacioEntreConyuges;

                // Dibuja el nodo del cónyuge
                DibujarNodoSimple(persona.Conyuge, xConyuge, y);

                // Línea horizontal conectando los cónyuges
                DibujarLineaConexion(xNodoPrincipal + nodoAncho / 2, y + nodoAlto / 2, 
                                    xConyuge - nodoAncho / 2, y + nodoAlto / 2, 
                                    Color.FromRgb(129, 199, 132));
            }

            // Dibuja el nodo principal
            DibujarNodoSimple(persona, xNodoPrincipal, y);

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
                DibujarLineaConexion(xCentroPareja, y + nodoAlto, xHijo, y + espacioVertical, 
                                    Color.FromRgb(100, 181, 246));

                // Dibuja hijo recursivamente
                DibujarNodoYDescendientes(hijo, xHijo, y + espacioVertical);
                xInicio += anchoHijo;
            }

            return Math.Max(totalAnchoHijos, anchoTotal);
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

        // Método antiguo - redirigir al nuevo método
        private double DibujarNodo(NodoFamiliar persona, double x, double y)
        {
            return DibujarNodoYDescendientes(persona, x, y);
        }

        // Calcula el ancho necesario para un nodo y sus descendientes
        private double CalcularAncho(NodoFamiliar persona)
        {
            // Ancho base: si tiene cónyuge, necesita espacio para ambos
            double anchoBase = nodoAncho + espacioHorizontal;
            if (persona.Conyuge != null)
            {
                anchoBase = (nodoAncho * 2 + 100) + espacioHorizontal; // Dos nodos + espacio entre ellos (actualizado a 100)
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
