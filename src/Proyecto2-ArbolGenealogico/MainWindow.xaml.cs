using Proyecto2_ArbolGenealogico.DataStructures;
using Proyecto2_ArbolGenealogico.Helpers;
using Proyecto2_ArbolGenealogico.Services;
using Proyecto2_ArbolGenealogico.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto2_ArbolGenealogico
{
    public partial class MainWindow : Window
    {
        private SistemaFamiliar sistema;
        private ArbolView arbolView;
        private MapaService mapaService;

        public MainWindow()
        {
            InitializeComponent();
            sistema = new SistemaFamiliar();
            arbolView = new ArbolView(ArbolCanvas);
            mapaService = new MapaService(MapaResidencias);
            mapaService.ConfigurarOverlayCanvas(CanvasOverlayMapa);
            
            lblPadre.Visibility = Visibility.Collapsed; // Ocultar inicialmente
            cmbPadre.Visibility = Visibility.Collapsed;
            chkEsPadreDeRaiz.Visibility = Visibility.Collapsed;
            lblConyuge.Visibility = Visibility.Collapsed;
            cmbConyuge.Visibility = Visibility.Collapsed;
            
            ActualizarListaPadres();
            mapaService.ConfigurarMapa();
        }
        private void ActualizarEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            if (!sistema.Arbol.TieneRaiz())
            {
                txtLejos.Text = "No hay familiares suficientes.";
                txtCerca.Text = "";
                txtPromedio.Text = "";
                return;
            }

            var lista = sistema.Arbol.ObtenerTodos();
            int n = lista.Largo();

            if (n < 2)
            {
                txtLejos.Text = "Se necesitan al menos 2 familiares.";
                txtCerca.Text = "";
                txtPromedio.Text = "";
                return;
            }

            double maxDist = -1;
            double minDist = double.MaxValue;
            double sumaDist = 0;
            int conteo = 0;

            NodoFamiliar lejosA = null, lejosB = null;
            NodoFamiliar cercaA = null, cercaB = null;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    var f1 = lista.Obtener(i);
                    var f2 = lista.Obtener(j);

                    // Se usa Haversine para calcular distancia
                    double d = MapaService.CalcularDistanciaHaversine(
                        f1.Latitud, f1.Longitud,
                        f2.Latitud, f2.Longitud
                    );

                    sumaDist += d;
                    conteo++;

                    if (d > maxDist)
                    {
                        maxDist = d;
                        lejosA = f1;
                        lejosB = f2;
                    }

                    if (d < minDist)
                    {
                        minDist = d;
                        cercaA = f1;
                        cercaB = f2;
                    }
                }
            }

            double promedio = sumaDist / conteo;

            // Imprimir resultados
            txtLejos.Text =
                $"Par más lejos:\n{lejosA.Nombre} ↔ {lejosB.Nombre}\nDistancia: {maxDist:F2} km";

            txtCerca.Text =
                $"Par más cerca:\n{cercaA.Nombre} ↔ {cercaB.Nombre}\nDistancia: {minDist:F2} km";

            txtPromedio.Text =
                $"Distancia promedio entre familiares:\n{promedio:F2} km";
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
                    // Agregar el nombre completo
                    cmbPadre.Items.Add(miembro.Nombre);
                }
                
                // Mostrar los controles solo cuando hay una raíz
                lblPadre.Visibility = Visibility.Visible;
                cmbPadre.Visibility = Visibility.Visible;
                chkEsPadreDeRaiz.Visibility = Visibility.Visible;
                lblConyuge.Visibility = Visibility.Visible;
                cmbConyuge.Visibility = Visibility.Visible;
            }
            else
            {
                // Ocultar los controles si no hay raíz
                lblPadre.Visibility = Visibility.Collapsed;
                cmbPadre.Visibility = Visibility.Collapsed;
                chkEsPadreDeRaiz.Visibility = Visibility.Collapsed;
                lblConyuge.Visibility = Visibility.Collapsed;
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
                    // Solo agregar miembros que no tengan cónyuge
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

            // Validaciones usando ValidacionHelper
            if (!ValidacionHelper.ValidarCamposRequeridos(nombre, cedula, fechaNacimiento, edadTexto, latitudTexto, longitudTexto, out string mensajeCampos))
            {
                MessageBox.Show(mensajeCampos, "Campo Requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidacionHelper.ValidarCedula(cedula, out string mensajeCedula))
            {
                MessageBox.Show(mensajeCedula, "Cédula Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (sistema.Arbol.BuscarPorCedula(cedula) != null)
            {
                MessageBox.Show($"Ya existe un familiar con la cédula '{cedula}'.\nCada miembro debe tener una cédula única.", 
                    "Cédula Duplicada", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ValidacionHelper.ValidarFecha(fechaNacimiento, out DateTime fechaNac))
            {
                MessageBox.Show("Por favor, ingresa la fecha de nacimiento en formato dd/MM/yyyy (ejemplo: 15/03/1990).", 
                    "Fecha Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            if (!ValidacionHelper.ValidarEdad(fechaNac, edad, out string mensajeEdad))
            {
                int edadCalculada = DateTime.Now.Year - fechaNac.Year;
                if (fechaNac.Date > DateTime.Now.AddYears(-edadCalculada))
                    edadCalculada--;
                    
                MessageBox.Show($"La edad ingresada ({edad}) no coincide con la fecha de nacimiento ({fechaNacimiento}).\n" +
                    $"La edad correcta basada en tu fecha de nacimiento es {edadCalculada} años.\n\n" +
                    $"Fecha actual: {DateTime.Now:dd/MM/yyyy}", 
                    "Edad Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidacionHelper.ValidarCoordenadas(latitudTexto, longitudTexto, out double latitud, out double longitud, out string mensajeCoordenadas))
            {
                MessageBox.Show(mensajeCoordenadas, "Coordenada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        if (ValidacionHelper.ValidarFecha(padre.FechaNacimiento, out DateTime fechaNacPadre))
                        {
                            if (fechaNac <= fechaNacPadre)
                            {
                                MessageBox.Show($"El hijo no puede nacer antes o el mismo día que su padre.\n" +
                                    $"Fecha de nacimiento del padre: {padre.FechaNacimiento}\n" +
                                    $"Fecha de nacimiento del hijo: {fechaNacimiento}", 
                                    "Fecha Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // Validar diferencia de edad mínima usando ValidacionHelper
                            if (!ValidacionHelper.ValidarDiferenciaEdadPadreHijo(fechaNacPadre, fechaNac, out string mensajeDiferencia))
                            {
                                MessageBox.Show(mensajeDiferencia, "Diferencia de Edad Inválida", 
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
                arbolView.DibujarArbol(sistema.Arbol);
                ActualizarMapa(); // Actualizar mapa con nuevos marcadores
                ActualizarEstadisticas_Click(null, null);
                LimpiarCampos();
            }
        }

        // Actualizar marcadores en el mapa
        private void ActualizarMapa()
        {
            if (sistema.Arbol.TieneRaiz())
            {
                sistema.Grafo.ConstruirDesdeArbol(sistema.Arbol);
                mapaService.MostrarFamiliaresEnMapa(sistema.Grafo);
            }
            else
            {
                // Si no hay raíz, limpiar el mapa completamente
                mapaService.LimpiarMapaCompleto();
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
                arbolView.DibujarArbol(sistema.Arbol);
                ActualizarMapa(); // Actualizar mapa después de eliminar
                ActualizarEstadisticas_Click(null, null);
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
    }
}

