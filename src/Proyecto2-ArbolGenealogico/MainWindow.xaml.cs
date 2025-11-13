////using System.Text;
////using System.Windows;
////using System.Windows.Controls;
////using System.Windows.Data;
////using System.Windows.Documents;
////using System.Windows.Input;
////using System.Windows.Media;
////using System.Windows.Media.Imaging;
////using System.Windows.Navigation;
////using System.Windows.Shapes;

//namespace Proyecto2_ArbolGenealogico
//{
//    public partial class MainWindow : Window
//    {
//        public MainWindow()
//        {
//            InitializeComponent();
//        }
//    }
//}
using Proyecto2_ArbolGenealogico.DataStructures;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Proyecto2_ArbolGenealogico
{
    public partial class MainWindow : Window
    {
        private SistemaFamiliar sistema;
        private double nodoAncho = 80;
        private double nodoAlto = 80;
        private double espacioHorizontal = 60;
        private double espacioVertical = 120;

        public MainWindow()
        {
            InitializeComponent();
            sistema = new SistemaFamiliar();
        }

        // BOTÓN AGREGAR
        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string padreNombre = txtPadre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Por favor, ingresa un nombre.");
                return;
            }

            // Crear el nuevo nodo familiar
            NodoFamiliar nuevo = new NodoFamiliar(
                nombre,
                "CédulaGenérica",
                "01/01/2000",
                25,
                "",
                0, 0
            );

            if (!sistema.Arbol.TieneRaiz())
            {
                sistema.Arbol.CrearRaiz(nuevo);
                MessageBox.Show($"'{nombre}' ha sido creado como raíz del árbol.");
            }
            else
            {
                bool agregado = false;

                if (!string.IsNullOrWhiteSpace(padreNombre))
                {
                    agregado = sistema.Arbol.AgregarMiembro(padreNombre, nuevo);
                }

                if (!agregado)
                {
                    MessageBox.Show($"No se encontró el padre '{padreNombre}'. Se agregó como hijo de la raíz.");
                    sistema.Arbol.Raiz.AgregarHijo(nuevo);
                }
            }

            DibujarArbol();
            LimpiarCampos();
        }
        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtCiudad.Clear();
            txtPais.Clear();
            txtPadre.Clear();
        }

        // 🔹 Dibuja el árbol completo en el Canvas
        private void DibujarArbol()
        {
            ArbolCanvas.Children.Clear();

            if (!sistema.Arbol.TieneRaiz())
                return;

            double startX = 400; // centro del canvas
            double startY = 40;
            DibujarNodo(sistema.Arbol.Raiz, startX, startY);
        }

        private double DibujarNodo(NodoFamiliar persona, double x, double y)
        {
            // Dibuja el nodo
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
                FontSize = 14,
                TextAlignment = TextAlignment.Center
            };

            Canvas.SetLeft(nodo, x - nodoAncho / 2);
            Canvas.SetTop(nodo, y);
            Canvas.SetLeft(texto, x - nodoAncho / 2 + 10);
            Canvas.SetTop(texto, y + nodoAlto / 2 - 10);

            ArbolCanvas.Children.Add(nodo);
            ArbolCanvas.Children.Add(texto);

            if (persona.Hijos.Largo() == 0)
                return nodoAncho;

            // Calcular ancho total de los hijos
            double totalAncho = 0;
            for (int i = 0; i < persona.Hijos.Largo(); i++)
                totalAncho += CalcularAncho(persona.Hijos.Obtener(i));

            double xInicio = x - totalAncho / 2;

            for (int i = 0; i < persona.Hijos.Largo(); i++)
            {
                var hijo = persona.Hijos.Obtener(i);
                double anchoHijo = CalcularAncho(hijo);
                double xHijo = xInicio + anchoHijo / 2;

                // Línea de conexión
                Line linea = new Line
                {
                    X1 = x,
                    Y1 = y + nodoAlto,
                    X2 = xHijo,
                    Y2 = y + nodoAlto + espacioVertical - 40,
                    Stroke = new SolidColorBrush(Color.FromRgb(100, 181, 246)),
                    StrokeThickness = 2
                };
                ArbolCanvas.Children.Add(linea);

                // Dibuja hijo recursivamente
                DibujarNodo(hijo, xHijo, y + espacioVertical);
                xInicio += anchoHijo;
            }

            return totalAncho;
        }

        private double CalcularAncho(NodoFamiliar persona)
        {
            if (persona.Hijos.Largo() == 0)
                return nodoAncho + espacioHorizontal;

            double ancho = 0;
            for (int i = 0; i < persona.Hijos.Largo(); i++)
                ancho += CalcularAncho(persona.Hijos.Obtener(i));

            return Math.Max(ancho, nodoAncho + espacioHorizontal);
        }
    }
}

