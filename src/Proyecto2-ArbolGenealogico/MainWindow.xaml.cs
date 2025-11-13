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
            chkEsPadreDeRaiz.Visibility = Visibility.Collapsed; // Ocultar inicialmente
            ActualizarListaPadres();
        }

        // Actualiza la lista de padres disponibles en el ComboBox
        private void ActualizarListaPadres()
        {
            cmbPadre.Items.Clear();
            
            if (sistema.Arbol.TieneRaiz())
            {
                var todosLosMiembros = sistema.Arbol.ObtenerTodos();
                foreach (var miembro in todosLosMiembros)
                {
                    // Agregar el nombre completo (se puede personalizar con más info)
                    cmbPadre.Items.Add(miembro.Nombre);
                }
                
                // Mostrar el checkbox de "padre de raíz" solo cuando hay una raíz
                chkEsPadreDeRaiz.Visibility = Visibility.Visible;
            }
            else
            {
                // Ocultar el checkbox si no hay raíz
                chkEsPadreDeRaiz.Visibility = Visibility.Collapsed;
            }

            // Mensaje informativo si no hay padres disponibles
            if (cmbPadre.Items.Count == 0)
            {
                cmbPadre.Items.Add("(Sin familiares en el árbol)");
                cmbPadre.IsEnabled = false;
            }
            else
            {
                cmbPadre.IsEnabled = true;
            }
        }

        // Manejar cuando se marca el checkbox de "padre de raíz"
        private void ChkEsPadreDeRaiz_Checked(object sender, RoutedEventArgs e)
        {
            // Deshabilitar el ComboBox de padre cuando se marca esta opción
            cmbPadre.IsEnabled = false;
            cmbPadre.SelectedIndex = -1;
            cmbPadre.Text = "";
        }

        // Manejar cuando se desmarca el checkbox de "padre de raíz"
        private void ChkEsPadreDeRaiz_Unchecked(object sender, RoutedEventArgs e)
        {
            // Habilitar el ComboBox de padre nuevamente
            if (sistema.Arbol.TieneRaiz())
            {
                cmbPadre.IsEnabled = true;
            }
        }

        // BOTÓN AGREGAR
        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            // Validar campos obligatorios
            string nombre = txtNombre.Text.Trim();
            string cedula = txtCedula.Text.Trim();
            string fechaNacimiento = txtFechaNacimiento.Text.Trim();
            string edadTexto = txtEdad.Text.Trim();
            string latitudTexto = txtLatitud.Text.Trim();
            string longitudTexto = txtLongitud.Text.Trim();
            string fotoRuta = txtFotoRuta.Text.Trim();
            string padreNombre = cmbPadre.Text.Trim();

            // Validaciones
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Por favor, ingresa el nombre completo.", "Campo Requerido", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(cedula))
            {
                MessageBox.Show("Por favor, ingresa la cédula.", "Campo Requerido", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar que la cédula sea solo números
            foreach (char c in cedula)
            {
                if (c < '0' || c > '9')
                {
                    MessageBox.Show("La cédula solo debe contener números.", "Cédula Inválida", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Validar que la cédula no esté duplicada
            if (sistema.Arbol.BuscarPorCedula(cedula) != null)
            {
                MessageBox.Show($"Ya existe un familiar con la cédula '{cedula}'.\nCada miembro debe tener una cédula única.", 
                    "Cédula Duplicada", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(fechaNacimiento))
            {
                MessageBox.Show("Por favor, ingresa la fecha de nacimiento.", "Campo Requerido", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar formato de fecha de nacimiento (dd/MM/yyyy)
            if (!DateTime.TryParseExact(fechaNacimiento, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime fechaNac))
            {
                MessageBox.Show("Por favor, ingresa la fecha de nacimiento en formato dd/MM/yyyy (ejemplo: 15/03/1990).", "Fecha Inválida", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (fechaNac > DateTime.Now)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser en el futuro.", "Fecha Inválida", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(edadTexto, out int edad) || edad < 0 || edad > 150)
            {
                MessageBox.Show("Por favor, ingresa una edad válida (0-150).", "Edad Inválida", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar que la edad coincida exactamente con la fecha de nacimiento
            int edadCalculada = DateTime.Now.Year - fechaNac.Year;
            // Ajustar si el cumpleaños aún no ha pasado este año
            if (fechaNac.Date > DateTime.Now.AddYears(-edadCalculada)) 
                edadCalculada--;
            
            if (edad != edadCalculada)
            {
                MessageBox.Show($"La edad ingresada ({edad}) no coincide con la fecha de nacimiento ({fechaNacimiento}).\n" +
                    $"La edad correcta basada en tu fecha de nacimiento es {edadCalculada} años.\n\n" +
                    $"Fecha actual: {DateTime.Now:dd/MM/yyyy}", 
                    "Edad Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(latitudTexto, out double latitud) || latitud < -90 || latitud > 90)
            {
                MessageBox.Show("Por favor, ingresa una latitud válida (-90 a 90).", "Coordenada Inválida", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(longitudTexto, out double longitud) || longitud < -180 || longitud > 180)
            {
                MessageBox.Show("Por favor, ingresa una longitud válida (-180 a 180).", "Coordenada Inválida", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar si se está agregando un padre a la raíz
            bool esPadreDeRaiz = chkEsPadreDeRaiz.IsChecked == true;

            // Si es padre de raíz, validar con el nodo raíz actual
            if (esPadreDeRaiz && sistema.Arbol.TieneRaiz())
            {
                var raizActual = sistema.Arbol.Raiz;
                
                // Validar que el nuevo padre sea mayor que la raíz actual
                if (raizActual.Edad >= edad)
                {
                    MessageBox.Show($"El padre debe ser mayor que la raíz actual del árbol.\n" +
                        $"Edad de la raíz actual '{raizActual.Nombre}': {raizActual.Edad} años\n" +
                        $"Edad del nuevo padre: {edad} años", 
                        "Edad Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar fechas de nacimiento
                if (!string.IsNullOrEmpty(raizActual.FechaNacimiento))
                {
                    if (DateTime.TryParseExact(raizActual.FechaNacimiento, "dd/MM/yyyy", 
                        System.Globalization.CultureInfo.InvariantCulture, 
                        System.Globalization.DateTimeStyles.None, out DateTime fechaNacRaiz))
                    {
                        if (fechaNac >= fechaNacRaiz)
                        {
                            MessageBox.Show($"El padre debe nacer antes que la raíz actual.\n" +
                                $"Fecha de nacimiento de la raíz: {raizActual.FechaNacimiento}\n" +
                                $"Fecha de nacimiento del padre: {fechaNacimiento}", 
                                "Fecha Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Validar que el padre tenga al menos 10 años cuando nace la raíz
                        int añosDiferencia = fechaNacRaiz.Year - fechaNac.Year;
                        if (añosDiferencia < 10)
                        {
                            MessageBox.Show($"El padre debe tener al menos 10 años cuando nace su hijo.\n" +
                                $"Diferencia de edad: {añosDiferencia} años", 
                                "Diferencia de Edad Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
            }
            // Validar relación padre-hijo (si hay padre especificado y NO es padre de raíz)
            else if (!esPadreDeRaiz && !string.IsNullOrWhiteSpace(padreNombre) && padreNombre != "(Sin familiares en el árbol)")
            {
                var padre = sistema.Arbol.BuscarPorNombre(padreNombre);
                if (padre != null)
                {
                    // Validar que el hijo sea más joven que el padre
                    if (padre.Edad <= edad)
                    {
                        MessageBox.Show($"El hijo no puede tener la misma edad o ser mayor que su padre.\n" +
                            $"Edad del padre '{padre.Nombre}': {padre.Edad} años\n" +
                            $"Edad del hijo: {edad} años", 
                            "Edad Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Validar fechas de nacimiento (hijo debe nacer después del padre)
                    if (!string.IsNullOrEmpty(padre.FechaNacimiento))
                    {
                        if (DateTime.TryParseExact(padre.FechaNacimiento, "dd/MM/yyyy", 
                            System.Globalization.CultureInfo.InvariantCulture, 
                            System.Globalization.DateTimeStyles.None, out DateTime fechaNacPadre))
                        {
                            if (fechaNac <= fechaNacPadre)
                            {
                                MessageBox.Show($"El hijo no puede nacer antes o el mismo día que su padre.\n" +
                                    $"Fecha de nacimiento del padre: {padre.FechaNacimiento}\n" +
                                    $"Fecha de nacimiento del hijo: {fechaNacimiento}", 
                                    "Fecha Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // Validar que el padre tenga al menos 10 años cuando nace el hijo
                            int añosDiferencia = fechaNac.Year - fechaNacPadre.Year;
                            if (añosDiferencia < 10)
                            {
                                MessageBox.Show($"El padre debe tener al menos 10 años cuando nace el hijo.\n" +
                                    $"Diferencia de edad: {añosDiferencia} años", 
                                    "Diferencia de Edad Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                }
            }

            // Crear el nuevo nodo familiar con todos los datos
            NodoFamiliar nuevo = new NodoFamiliar(
                nombre,
                cedula,
                fechaNacimiento,
                edad,
                fotoRuta,
                latitud,
                longitud
            );

            // Agregar al sistema (árbol + grafo)
            bool exito = false;
            
            // Caso 1: Agregar un padre a la raíz actual
            if (esPadreDeRaiz && sistema.Arbol.TieneRaiz())
            {
                string nombreRaizAnterior = sistema.Arbol.Raiz.Nombre;
                sistema.Arbol.AgregarPadreARaiz(nuevo);
                exito = true;
                MessageBox.Show($"'{nombre}' ha sido agregado como padre de '{nombreRaizAnterior}' (antigua raíz).\n" +
                    $"'{nombre}' es ahora la nueva raíz del árbol.", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Caso 2: Crear la primera raíz del árbol
            else if (!sistema.Arbol.TieneRaiz())
            {
                exito = sistema.AgregarMiembroCompleto("", nuevo);
                MessageBox.Show($"'{nombre}' ha sido creado como raíz del árbol.", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Caso 3: Agregar un hijo a un padre existente
            else
            {
                if (string.IsNullOrWhiteSpace(padreNombre) || padreNombre == "(Sin familiares en el árbol)")
                {
                    MessageBox.Show("Por favor, especifica el nombre del padre para agregar este miembro.", 
                        "Padre Requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                exito = sistema.AgregarMiembroCompleto(padreNombre, nuevo);
                
                if (exito)
                {
                    MessageBox.Show($"'{nombre}' ha sido agregado como hijo de '{padreNombre}'.", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"No se encontró el padre '{padreNombre}'. Verifica el nombre e intenta nuevamente.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (exito)
            {
                chkEsPadreDeRaiz.IsChecked = false; // Desmarcar el checkbox
                ActualizarListaPadres(); // Actualizar lista de padres disponibles
                DibujarArbol();
                LimpiarCampos();
            }
        }

        // Botón para explorar y seleccionar foto
        private void ExplorarFoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Seleccionar fotografía",
                Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txtFotoRuta.Text = openFileDialog.FileName;
            }
        }

        // Botón limpiar campos
        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtCedula.Clear();
            txtFechaNacimiento.Clear();
            txtEdad.Clear();
            txtFotoRuta.Clear();
            txtLatitud.Clear();
            txtLongitud.Clear();
            cmbPadre.SelectedIndex = -1;
            cmbPadre.Text = "";
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
                TextAlignment = TextAlignment.Center,
                Width = nodoAncho,
                TextWrapping = TextWrapping.Wrap
            };

            Canvas.SetLeft(nodo, x - nodoAncho / 2);
            Canvas.SetTop(nodo, y);
            Canvas.SetLeft(texto, x - nodoAncho / 2);
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

                // Línea de conexión (desde borde inferior del padre hasta borde superior del hijo)
                Line linea = new Line
                {
                    X1 = x,
                    Y1 = y + nodoAlto,  // Borde inferior del padre
                    X2 = xHijo,
                    Y2 = y + espacioVertical,  // Borde superior del hijo
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

