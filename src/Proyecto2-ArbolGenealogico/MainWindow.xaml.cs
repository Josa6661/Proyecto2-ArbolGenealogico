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
        private double nodoAncho = 120;
        private double nodoAlto = 100;
        private double espacioHorizontal = 80;
        private double espacioVertical = 140;

        public MainWindow()
        {
            InitializeComponent();
            sistema = new SistemaFamiliar();
            chkEsPadreDeRaiz.Visibility = Visibility.Collapsed; // Ocultar inicialmente
            cmbConyuge.Visibility = Visibility.Collapsed; // Ocultar inicialmente
            ActualizarListaPadres();
        }

        // Actualiza la lista de padres disponibles en el ComboBox
        private void ActualizarListaPadres()
        {
            cmbPadre.Items.Clear();
            
            if (sistema.Arbol.TieneRaiz())
            {
                // Solo obtener nodos jerárquicos (sin cónyuges) para la lista de padres
                var nodosJerarquicos = sistema.Arbol.ObtenerNodosJerarquicos();
                for (int i = 0; i < nodosJerarquicos.Largo(); i++)
                {
                    var miembro = nodosJerarquicos.Obtener(i);
                    // Agregar el nombre completo (se puede personalizar con más info)
                    cmbPadre.Items.Add(miembro.Nombre);
                }
                
                // Mostrar los controles solo cuando hay una raíz
                chkEsPadreDeRaiz.Visibility = Visibility.Visible;
                cmbConyuge.Visibility = Visibility.Visible;
            }
            else
            {
                // Ocultar los controles si no hay raíz
                chkEsPadreDeRaiz.Visibility = Visibility.Collapsed;
                cmbConyuge.Visibility = Visibility.Collapsed;
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
            
            ActualizarListaConyuges();
            ActualizarListaEliminar();
        }
        
        // Actualiza la lista de miembros disponibles para agregar cónyuge
        private void ActualizarListaConyuges()
        {
            cmbConyuge.Items.Clear();
            
            if (sistema.Arbol.TieneRaiz())
            {
                var todosLosMiembros = sistema.Arbol.ObtenerTodos();
                for (int i = 0; i < todosLosMiembros.Largo(); i++)
                {
                    var miembro = todosLosMiembros.Obtener(i);
                    // Solo agregar miembros que NO tengan cónyuge
                    if (miembro.Conyuge == null)
                    {
                        cmbConyuge.Items.Add(miembro.Nombre);
                    }
                }
            }

            // Mensaje informativo si no hay miembros disponibles
            if (cmbConyuge.Items.Count == 0)
            {
                cmbConyuge.Items.Add("(Sin miembros disponibles)");
                cmbConyuge.IsEnabled = false;
            }
            else
            {
                cmbConyuge.IsEnabled = true;
            }
        }

        // Actualiza la lista de miembros disponibles para eliminar
        private void ActualizarListaEliminar()
        {
            cmbEliminar.Items.Clear();
            
            if (sistema.Arbol.TieneRaiz())
            {
                var todosLosMiembros = sistema.Arbol.ObtenerTodos();
                for (int i = 0; i < todosLosMiembros.Largo(); i++)
                {
                    var miembro = todosLosMiembros.Obtener(i);
                    cmbEliminar.Items.Add(miembro.Nombre);
                }
            }

            if (cmbEliminar.Items.Count == 0)
            {
                cmbEliminar.Items.Add("(Sin miembros en el árbol)");
                cmbEliminar.IsEnabled = false;
            }
            else
            {
                cmbEliminar.IsEnabled = true;
            }
        }

        // Manejar cuando se marca el checkbox de "padre de raíz"
        private void ChkEsPadreDeRaiz_Checked(object sender, RoutedEventArgs e)
        {
            // Deshabilitar el ComboBox de padre cuando se marca esta opción
            cmbPadre.IsEnabled = false;
            cmbPadre.SelectedIndex = -1;
            cmbPadre.Text = "";
            // Limpiar el ComboBox de cónyuge
            cmbConyuge.SelectedIndex = -1;
            cmbConyuge.Text = "";
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

        // Manejar cuando se selecciona un padre
        private void CmbPadre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si se selecciona un padre válido, limpiar el cónyuge
            if (cmbPadre.SelectedIndex >= 0 && cmbPadre.SelectedItem != null)
            {
                string seleccion = cmbPadre.SelectedItem.ToString();
                if (!string.IsNullOrWhiteSpace(seleccion) && seleccion != "(Sin familiares en el árbol)")
                {
                    cmbConyuge.SelectedIndex = -1;
                    cmbConyuge.Text = "";
                }
            }
        }

        // Manejar cuando se selecciona un cónyuge
        private void CmbConyuge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si se selecciona un cónyuge válido, limpiar el padre y desmarcar checkbox de padre de raíz
            if (cmbConyuge.SelectedIndex >= 0 && cmbConyuge.SelectedItem != null)
            {
                string seleccion = cmbConyuge.SelectedItem.ToString();
                if (!string.IsNullOrWhiteSpace(seleccion) && seleccion != "(Sin miembros disponibles)")
                {
                    cmbPadre.SelectedIndex = -1;
                    cmbPadre.Text = "";
                    chkEsPadreDeRaiz.IsChecked = false;
                }
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

            // Validar si se está agregando un padre a la raíz o un cónyuge
            bool esPadreDeRaiz = chkEsPadreDeRaiz.IsChecked == true;
            string conyugeNombre = cmbConyuge.Text.Trim();
            bool esConyuge = !string.IsNullOrWhiteSpace(conyugeNombre) && conyugeNombre != "(Sin miembros disponibles)";

            // Si es cónyuge, validar que se haya seleccionado un miembro
            if (esConyuge)
            {
                var miembro = sistema.Arbol.BuscarPorNombre(conyugeNombre);
                if (miembro == null)
                {
                    MessageBox.Show($"No se encontró el miembro '{conyugeNombre}'.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (miembro.Conyuge != null)
                {
                    MessageBox.Show($"El miembro '{conyugeNombre}' ya tiene un cónyuge: '{miembro.Conyuge.Nombre}'.", 
                        "Ya Tiene Cónyuge", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // No validar edad estricta entre cónyuges, pero sí que sean edades razonables
                // (por ejemplo, no más de 30 años de diferencia)
                int diferenciaEdad = Math.Abs(miembro.Edad - edad);
                if (diferenciaEdad > 30)
                {
                    var result = MessageBox.Show($"Hay una diferencia de {diferenciaEdad} años entre los cónyuges.\n" +
                        $"¿Deseas continuar de todas formas?", 
                        "Diferencia de Edad Grande", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                        return;
                }
            }
            // Si es padre de raíz, validar con el nodo raíz actual
            else if (esPadreDeRaiz && sistema.Arbol.TieneRaiz())
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
            
            // Caso 1: Agregar un cónyuge a un miembro existente
            if (esConyuge)
            {
                exito = sistema.Arbol.AgregarConyuge(conyugeNombre, nuevo);
                if (exito)
                {
                    MessageBox.Show($"'{nombre}' ha sido agregado como cónyuge de '{conyugeNombre}'.", "Éxito", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"No se pudo agregar el cónyuge. Verifica que '{conyugeNombre}' no tenga ya un cónyuge.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            // Caso 2: Agregar un padre a la raíz actual
            else if (esPadreDeRaiz && sistema.Arbol.TieneRaiz())
            {
                string nombreRaizAnterior = sistema.Arbol.Raiz.Nombre;
                sistema.Arbol.AgregarPadreARaiz(nuevo);
                exito = true;
                MessageBox.Show($"'{nombre}' ha sido agregado como padre de '{nombreRaizAnterior}' (antigua raíz).\n" +
                    $"'{nombre}' es ahora la nueva raíz del árbol.", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Caso 3: Crear la primera raíz del árbol
            else if (!sistema.Arbol.TieneRaiz())
            {
                exito = sistema.AgregarMiembroCompleto("", nuevo);
                MessageBox.Show($"'{nombre}' ha sido creado como raíz del árbol.", "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Caso 4: Agregar un hijo a un padre existente
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
                ActualizarListaPadres(); // Actualizar listas de padres y cónyuges disponibles
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

        // Botón eliminar miembro
        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            string nombreEliminar = cmbEliminar.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreEliminar) || nombreEliminar == "(Sin miembros en el árbol)")
            {
                MessageBox.Show("Por favor, selecciona un miembro para eliminar.", 
                    "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Confirmar eliminación
            var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar a '{nombreEliminar}'?\n" +
                "Esta acción no se puede deshacer.\n\n" +
                "NOTA: Si eliminas un nodo con hijos, todos sus descendientes también serán eliminados.", 
                "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
                return;

            // Buscar el miembro
            var miembro = sistema.Arbol.BuscarPorNombre(nombreEliminar);
            if (miembro == null)
            {
                MessageBox.Show($"No se encontró el miembro '{nombreEliminar}'.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool exito = false;

            // Verificar si es un cónyuge (no está en la jerarquía del árbol)
            bool esConyuge = false;
            var nodosJerarquicos = sistema.Arbol.ObtenerNodosJerarquicos();
            bool estaEnJerarquia = false;
            for (int i = 0; i < nodosJerarquicos.Largo(); i++)
            {
                if (nodosJerarquicos.Obtener(i).Nombre == nombreEliminar)
                {
                    estaEnJerarquia = true;
                    break;
                }
            }

            if (!estaEnJerarquia && miembro.Conyuge != null)
            {
                // Es un cónyuge, solo eliminar la relación
                esConyuge = true;
                miembro.Conyuge.Conyuge = null;
                exito = true;
            }
            else
            {
                // Es un nodo en la jerarquía
                // Si el miembro tiene cónyuge, eliminar la relación bidireccional
                if (miembro.Conyuge != null)
                {
                    miembro.Conyuge.Conyuge = null;
                }

                // Eliminar el miembro del árbol
                exito = sistema.Arbol.EliminarMiembro(nombreEliminar);
            }

            if (exito)
            {
                string mensaje = esConyuge 
                    ? $"'{nombreEliminar}' (cónyuge) ha sido eliminado." 
                    : $"'{nombreEliminar}' ha sido eliminado del árbol.";
                    
                MessageBox.Show(mensaje, "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                ActualizarListaPadres();
                DibujarArbol();
                cmbEliminar.SelectedIndex = -1;
                cmbEliminar.Text = "";
            }
            else
            {
                MessageBox.Show($"No se pudo eliminar a '{nombreEliminar}'.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            cmbConyuge.SelectedIndex = -1;
            cmbConyuge.Text = "";
        }

        // 🔹 Dibuja el árbol completo en el Canvas
        private void DibujarArbol()
        {
            ArbolCanvas.Children.Clear();

            if (!sistema.Arbol.TieneRaiz())
                return;

            // Calcular el ancho total del árbol para centrar y dimensionar el Canvas
            double anchoArbol = CalcularAncho(sistema.Arbol.Raiz);
            double alturaArbol = CalcularAltura(sistema.Arbol.Raiz) * espacioVertical + nodoAlto + 100;
            
            // Asegurar un tamaño mínimo del Canvas
            double anchoCanvas = Math.Max(anchoArbol + 400, 1200);
            double alturaCanvas = Math.Max(alturaArbol, 800);
            
            ArbolCanvas.Width = anchoCanvas;
            ArbolCanvas.Height = alturaCanvas;

            // Centrar el árbol en el Canvas
            double startX = anchoCanvas / 2;
            double startY = 60;
            DibujarNodo(sistema.Arbol.Raiz, startX, startY);
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

                ArbolCanvas.Children.Add(nodoConyuge);
                ArbolCanvas.Children.Add(textoConyuge);

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
                ArbolCanvas.Children.Add(lineaConyuge);
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

            ArbolCanvas.Children.Add(nodo);
            ArbolCanvas.Children.Add(texto);

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
                ArbolCanvas.Children.Add(linea);

                // Dibuja hijo recursivamente
                DibujarNodo(hijo, xHijo, y + espacioVertical);
                xInicio += anchoHijo;
            }

            return Math.Max(totalAnchoHijos, anchoTotal);
        }

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

