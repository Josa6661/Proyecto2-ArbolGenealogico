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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Proyecto2_ArbolGenealogico
{
    public partial class MainWindow : Window
    {
        private double posX = 50; // posición inicial X para el primer nodo
        private double posY = 50; // posición inicial Y
        private double separacionVertical = 120; // distancia entre nodos
        private double separacionHorizontal = 120;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text;
            string apellido = txtApellido.Text;

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                MessageBox.Show("Por favor, ingresa el nombre y apellido del familiar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Crear el nodo visual (círculo)
            Ellipse nodo = new Ellipse
            {
                Width = 80,
                Height = 80,
                Stroke = new SolidColorBrush(Color.FromRgb(100, 181, 246)), // #64B5F6
                StrokeThickness = 2
            };

            // Crear el texto (nombre del familiar)
            TextBlock texto = new TextBlock
            {
                Text = $"{nombre} {apellido}",
                Foreground = Brushes.White,
                FontSize = 14,
                TextAlignment = TextAlignment.Center
            };

            // Posicionar el nodo en el Canvas
            Canvas.SetLeft(nodo, posX);
            Canvas.SetTop(nodo, posY);

            // Centrar el texto dentro del nodo
            Canvas.SetLeft(texto, posX + 10);
            Canvas.SetTop(texto, posY + 30);

            // Agregar al Canvas
            ArbolCanvas.Children.Add(nodo);
            ArbolCanvas.Children.Add(texto);

            // Ajustar posición para el siguiente nodo
            posX += separacionHorizontal;

            // Limpiar los campos
            txtNombre.Clear();
            txtApellido.Clear();
            txtCiudad.Clear();
            txtPais.Clear();
        }
    }
}
