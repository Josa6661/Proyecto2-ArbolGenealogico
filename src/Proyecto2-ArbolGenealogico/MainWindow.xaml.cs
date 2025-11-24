using Proyecto2_ArbolGenealogico.DataStructures;
using Proyecto2_ArbolGenealogico.BusinessLogic;
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
            arbolView.OnNodoClick = CargarDatosNodoEnFormulario;
            mapaService = new MapaService(MapaResidencias);
            mapaService.ConfigurarOverlayCanvas(CanvasOverlayMapa);
            
            lblPadre.Visibility = Visibility.Collapsed; // Ocultar inicialmente
            cmbPadre.Visibility = Visibility.Collapsed;
            lblConyuge.Visibility = Visibility.Collapsed;
            cmbConyuge.Visibility = Visibility.Collapsed;
            lblHermano.Visibility = Visibility.Collapsed;
            cmbHermano.Visibility = Visibility.Collapsed;
            
            // Ocultar sección de agregar padres a miembro existente
            lblMiembroParaPadres.Visibility = Visibility.Collapsed;
            cmbMiembroParaPadres.Visibility = Visibility.Collapsed;
            btnAgregarPadresAMiembro.Visibility = Visibility.Collapsed;


            // Ocultar sección de eliminar hasta que se cree el primer miembro
            lblEliminar.Visibility = Visibility.Collapsed;
            cmbEliminar.Visibility = Visibility.Collapsed;
            pnlBotonesEliminar.Visibility = Visibility.Collapsed;
            
            ActualizarListaPadres();
            mapaService.ConfigurarMapa();
        }

        private void MostrarInstrucciones_Click(object sender, RoutedEventArgs e)
        {
            var ventanaInstrucciones = new InstruccionesWindow();
            ventanaInstrucciones.Owner = this;
            ventanaInstrucciones.ShowDialog();
        }

        private void CargarDatosNodoEnFormulario(NodoFamiliar nodo)
        {
            var ventanaEditar = new EditarNodoWindow(nodo);
            if (ventanaEditar.ShowDialog() == true)
            {
                arbolView.DibujarArbol(sistema.Arbol);
                ActualizarMapa();
                ActualizarListaPadres();
            }
        }
        private void ActualizarEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            var estadisticas = CalculadoraEstadisticas.CalcularEstadisticas(sistema.Arbol);

            if (!estadisticas.HayDatosSuficientes)
            {
                txtLejos.Text = estadisticas.MensajeError;
                txtCerca.Text = "";
                txtPromedio.Text = estadisticas.DistanciaPromedio > 0
                    ? $"Distancia promedio: {estadisticas.DistanciaPromedio:F2} km"
                    : "";
                return;
            }

            txtLejos.Text = $"Par más lejos:\n{estadisticas.ParLejanoA.Nombre} ↔ {estadisticas.ParLejanoB.Nombre}\nDistancia: {estadisticas.DistanciaMaxima:F2} km";
            txtCerca.Text = $"Par más cerca:\n{estadisticas.ParCercanoA.Nombre} ↔ {estadisticas.ParCercanoB.Nombre}\nDistancia: {estadisticas.DistanciaMinima:F2} km";
            txtPromedio.Text = $"Distancia promedio entre familiares:\n{estadisticas.DistanciaPromedio:F2} km";
        }
        // Actualiza la lista de padres disponibles en el ComboBox
        private void ActualizarListaPadres()
        {
            cmbPadre.Items.Clear();
            
            if (sistema.Arbol.TieneRaiz())
            {
                // Obtener TODOS los nodos (incluyendo cónyuges y padres añadidos) para la lista de padres
                var todosNodos = sistema.Arbol.ObtenerTodos();
                for (int i = 0; i < todosNodos.Largo(); i++)
                {
                    var miembro = todosNodos.Obtener(i);
                    // Agregar el nombre completo
                    cmbPadre.Items.Add(miembro.Nombre);
                }
                
                // Mostrar los controles solo cuando hay una raíz
                lblPadre.Visibility = Visibility.Visible;
                cmbPadre.Visibility = Visibility.Visible;
                lblConyuge.Visibility = Visibility.Visible;
                cmbConyuge.Visibility = Visibility.Visible;
                lblHermano.Visibility = Visibility.Visible;
                cmbHermano.Visibility = Visibility.Visible;

                // Mostrar sección de agregar padres a miembro existente
                lblMiembroParaPadres.Visibility = Visibility.Visible;
                cmbMiembroParaPadres.Visibility = Visibility.Visible;
                btnAgregarPadresAMiembro.Visibility = Visibility.Visible;

                // Mostrar también la sección de eliminar
                lblEliminar.Visibility = Visibility.Visible;
                cmbEliminar.Visibility = Visibility.Visible;
                pnlBotonesEliminar.Visibility = Visibility.Visible;
            }
            else
            {
                // Ocultar los controles si no hay raíz
                lblPadre.Visibility = Visibility.Collapsed;
                cmbPadre.Visibility = Visibility.Collapsed;
                lblConyuge.Visibility = Visibility.Collapsed;
                cmbConyuge.Visibility = Visibility.Collapsed;
                lblHermano.Visibility = Visibility.Collapsed;
                cmbHermano.Visibility = Visibility.Collapsed;
                
                // Ocultar sección de agregar padres a miembro existente
                lblMiembroParaPadres.Visibility = Visibility.Collapsed;
                cmbMiembroParaPadres.Visibility = Visibility.Collapsed;
                btnAgregarPadresAMiembro.Visibility = Visibility.Collapsed;

                // Ocultar también la sección de eliminar
                lblEliminar.Visibility = Visibility.Collapsed;
                cmbEliminar.Visibility = Visibility.Collapsed;
                pnlBotonesEliminar.Visibility = Visibility.Collapsed;
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
            ActualizarListaHermanos();
            ActualizarListaMiembrosParaPadres();
            ActualizarListaEliminar();
        }

        // Actualiza la lista de miembros disponibles para agregarles padres
        private void ActualizarListaMiembrosParaPadres()
        {
            cmbMiembroParaPadres.Items.Clear();

            if (sistema.Arbol.TieneRaiz())
            {
                var todosLosMiembros = sistema.Arbol.ObtenerTodos();
                for (int i = 0; i < todosLosMiembros.Largo(); i++)
                {
                    var miembro = todosLosMiembros.Obtener(i);
                    // Agregar todos los miembros que no tengan ya 2 padres
                    if (miembro.Padres.Largo() < 2)
                    {
                        string info = $"{miembro.Nombre}";
                        if (miembro.Padres.Largo() == 1)
                            info += $" (ya tiene 1 padre: {miembro.Padres.Obtener(0).Nombre})";
                        
                        cmbMiembroParaPadres.Items.Add(info);
                    }
                }
            }

            // Mensaje informativo si no hay miembros disponibles
            if (cmbMiembroParaPadres.Items.Count == 0)
            {
                cmbMiembroParaPadres.Items.Add("(Todos los miembros tienen 2 padres)");
                cmbMiembroParaPadres.IsEnabled = false;
                btnAgregarPadresAMiembro.IsEnabled = false;
            }
            else
            {
                cmbMiembroParaPadres.IsEnabled = true;
                btnAgregarPadresAMiembro.IsEnabled = true;
            }
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

        //Actualiza la lista de miembros disponibles para agregar como hermanos
        private void ActualizarListaHermanos()
        {
            cmbHermano.Items.Clear();

            if (sistema.Arbol.TieneRaiz())
            {
                // Obtener TODOS los miembros (incluyendo cónyuges) para poder agregar hermanos a cualquiera
                var todosLosMiembros = sistema.Arbol.ObtenerTodos();
                for (int i = 0; i < todosLosMiembros.Largo(); i++)
                {
                    var miembro = todosLosMiembros.Obtener(i);
                    // Solo agregar miembros que tengan padre (para poder agregar hermano)
                    if (miembro.Padres.Largo() > 0)
                    {
                        cmbHermano.Items.Add(miembro.Nombre);
                    }
                }
            }

            // Mensaje informativo si no hay miembros disponibles
            if (cmbHermano.Items.Count == 0)
            {
                cmbHermano.Items.Add("(Sin miembros con padres)");
                cmbHermano.IsEnabled = false;
            }
            else
            {
                cmbHermano.IsEnabled = true;
            }
        }

        //Manejar cuando se selecciona un hermano
        private void CmbHermano_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si se selecciona un hermano válido, limpiar padre, cónyuge y checkbox
            if (cmbHermano.SelectedIndex >= 0 && cmbHermano.SelectedItem != null)
            {
                string seleccion = cmbHermano.SelectedItem.ToString();
                if (!string.IsNullOrWhiteSpace(seleccion) && seleccion != "(Sin miembros con padres)")
                {
                    cmbPadre.SelectedIndex = -1;
                    cmbPadre.Text = "";
                    cmbConyuge.SelectedIndex = -1;
                    cmbConyuge.Text = "";
                }
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
                    cmbHermano.SelectedIndex = -1;
                    cmbHermano.Text = "";
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
                    cmbHermano.SelectedIndex = -1;
                    cmbHermano.Text = "";
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

            if (!ValidacionHelper.ValidarEdad(fechaNac, edad, out _))
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

            // Validar si se está agregando un cónyuge
            string conyugeNombre = cmbConyuge.Text.Trim();
            bool esConyuge = !string.IsNullOrWhiteSpace(conyugeNombre) && conyugeNombre != "(Sin miembros disponibles)";

            //Detectar si es hermano
            string hermanoNombre = cmbHermano.Text.Trim();
            bool esHermano = !string.IsNullOrWhiteSpace(hermanoNombre) && hermanoNombre != "(Sin miembros con padres)";


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
            // Validar relación padre-hijo (si hay padre especificado)
            if (!string.IsNullOrWhiteSpace(padreNombre) && padreNombre != "(Sin familiares en el árbol)")
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
                    if (!string.IsNullOrEmpty(padre.FechaNacimiento) &&
                        ValidacionHelper.ValidarFecha(padre.FechaNacimiento, out DateTime fechaNacPadre))
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
            // Validar relación de hermanos
            else if (esHermano)
            {
                var hermano = sistema.Arbol.BuscarPorNombre(hermanoNombre);
                if (hermano == null)
                {
                    MessageBox.Show($"No se encontró el miembro '{hermanoNombre}'.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (hermano.Padres.Largo() == 0)
                {
                    MessageBox.Show($"El miembro '{hermanoNombre}' no tiene padres asignados.\n" +
                        "No se puede agregar un hermano sin padre común.\n\n" +
                        "Sugerencia: Primero crea al padre y luego agrega a los hermanos.",
                        "Sin Padres", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // Validar edades (hermanos deben tener edades cercanas, pero no idénticas)
                int diferenciaEdad = Math.Abs(hermano.Edad - edad);

                // Advertir si la diferencia es muy grande (más de 25 años)
                if (diferenciaEdad > 25)
                {
                    var result = MessageBox.Show(
                        $"Hay una diferencia de {diferenciaEdad} años entre los hermanos:\n" +
                        $"• {hermanoNombre}: {hermano.Edad} años\n" +
                        $"• {nombre}: {edad} años\n\n" +
                        $"¿Estás seguro de que son hermanos? Podrían ser padre/hijo o tío/sobrino.\n" +
                        $"¿Deseas continuar de todas formas?",
                        "Diferencia de Edad Grande",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                // Validar fechas de nacimiento si están disponibles
                if (!string.IsNullOrEmpty(hermano.FechaNacimiento) &&
                    ValidacionHelper.ValidarFecha(hermano.FechaNacimiento, out DateTime fechaNacHermano))
                {
                    // Los hermanos no pueden nacer el mismo día (a menos que sean gemelos, pero eso es raro)
                    if (fechaNac == fechaNacHermano)
                    {
                        var result = MessageBox.Show(
                            $"Ambos hermanos tienen la misma fecha de nacimiento.\n" +
                            $"¿Son gemelos/mellizos?\n\n" +
                            $"Si no lo son, verifica las fechas.",
                            "Misma Fecha de Nacimiento",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.No)
                            return;
                    }

                    // Validar orden cronológico si hay diferencia significativa
                    TimeSpan diferenciaTiempo = (fechaNac - fechaNacHermano).Duration();
                    int mesesDiferencia = (int)(diferenciaTiempo.TotalDays / 30);

                    if (mesesDiferencia < 9 && fechaNac != fechaNacHermano)
                    {
                        MessageBox.Show(
                            $"Los hermanos tienen menos de 9 meses de diferencia:\n" +
                            $"• {hermanoNombre}: {hermano.FechaNacimiento}\n" +
                            $"• {nombre}: {fechaNacimiento}\n\n" +
                            $"Esto es biológicamente imposible (a menos que sean gemelos).\n" +
                            $"Por favor verifica las fechas.",
                            "Fechas Inconsistentes",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                }


                // Validar que el(los) padre(s) sea(n) mayor(es) que ambos hermanos
                for (int p = 0; p < hermano.Padres.Largo(); p++)
                {
                    var padre = hermano.Padres.Obtener(p);
                    if (padre.Edad <= edad)
                    {
                        MessageBox.Show(
                            $"El padre común debe ser mayor que ambos hermanos.\n" +
                            $"Edad del padre '{padre.Nombre}': {padre.Edad} años\n" +
                            $"Edad del nuevo hermano '{nombre}': {edad} años",
                            "Edad Inconsistente",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
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
                    // Agregar al grafo también
                    var nodoGrafo = new GrafoGeografico.NodoGrafo(nuevo.Cedula, nuevo.Nombre, nuevo.Latitud, nuevo.Longitud, nuevo.FotoRuta);
                    sistema.Grafo.AgregarNodo(nodoGrafo);
                    sistema.Grafo.RecalcularTodasDistancias();
                    
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
            // Caso 1.5: Agregar un hermano a un miembro existente
            else if (esHermano)
            {
                var hermano = sistema.Arbol.BuscarPorNombre(hermanoNombre);
                if (hermano != null && hermano.Padres.Largo() > 0)
                {
                    // Obtener el primer padre para usar el flujo estándar
                    var primerPadre = hermano.Padres.Obtener(0);
                    string nombrePadreComun = primerPadre.Nombre;
                    
                    // Usar el flujo estándar de agregar hijo (igual que "Seleccionar padre")
                    exito = sistema.AgregarMiembroCompleto(primerPadre.Nombre, nuevo);
                    
                    if (exito)
                    {
                        // Si el hermano tiene 2 padres, agregar el segundo también
                        if (hermano.Padres.Largo() == 2)
                        {
                            var segundoPadre = hermano.Padres.Obtener(1);
                            nuevo.AgregarPadre(segundoPadre);
                            segundoPadre.AgregarHijo(nuevo);
                            nombrePadreComun = $"{primerPadre.Nombre} y {segundoPadre.Nombre}";
                        }
                        
                        MessageBox.Show(
                            $"'{nombre}' ha sido agregado como hermano/hermana de '{hermanoNombre}'.\n" +
                            $"Ambos son hijos de {nombrePadreComun}.",
                            "Éxito",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            $"No se pudo agregar como hermano de '{hermanoNombre}'.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"No se encontró el hermano '{hermanoNombre}' o no tiene padres asignados.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            // Caso 2: Crear la primera raíz del árbol
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

                // Buscar el padre ANTES de agregar para manejar cónyuges correctamente
                var padreEncontrado = sistema.Arbol.BuscarPorNombre(padreNombre);
                if (padreEncontrado == null)
                {
                    MessageBox.Show($"No se encontró el padre '{padreNombre}'. Verifica el nombre e intenta nuevamente.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Si el padre tiene cónyuge, determinar cuál está en la jerarquía principal
                NodoFamiliar padreJerarquico = padreEncontrado;
                NodoFamiliar padreConyugal = null;
                
                if (padreEncontrado.Conyuge != null)
                {
                    // Obtener los nodos jerárquicos
                    var nodosJerarquicos = sistema.Arbol.ObtenerNodosJerarquicos();
                    bool padreEnJerarquia = false;
                    
                    // Verificar si el padre seleccionado está en la jerarquía
                    for (int i = 0; i < nodosJerarquicos.Largo(); i++)
                    {
                        if (nodosJerarquicos.Obtener(i).Cedula == padreEncontrado.Cedula)
                        {
                            padreEnJerarquia = true;
                            break;
                        }
                    }
                    
                    // Si el padre seleccionado NO está en la jerarquía, usar su cónyuge (que sí debe estar)
                    if (!padreEnJerarquia)
                    {
                        padreConyugal = padreEncontrado;  // El seleccionado será el segundo padre
                        padreJerarquico = padreEncontrado.Conyuge;  // El cónyuge es el jerárquico
                    }
                    else
                    {
                        padreConyugal = padreEncontrado.Conyuge;  // El cónyuge será el segundo padre
                    }
                }
                
                // Agregar al padre jerárquico
                exito = sistema.AgregarMiembroCompleto(padreJerarquico.Nombre, nuevo);

                if (exito)
                {
                    // Mostrar el mensaje con el nombre que el usuario seleccionó primero
                    string mensajeHijo = $"'{nombre}' ha sido agregado como hijo de '{padreNombre}'";
                    
                    // Si hay cónyuge, agregar como segundo padre
                    if (padreConyugal != null)
                    {
                        string nombreOtroPadre = padreConyugal.Nombre;
                        mensajeHijo = $"'{nombre}' ha sido agregado como hijo de '{padreNombre}' y '{nombreOtroPadre}'";
                        
                        if (nuevo.Padres.Largo() < 2)
                        {
                            nuevo.AgregarPadre(padreConyugal);
                            padreConyugal.AgregarHijo(nuevo);
                        }
                    }
                    mensajeHijo += ".";
                    
                    MessageBox.Show(mensajeHijo, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
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
                ActualizarListaPadres(); // Actualizar listas de padres y cónyuges disponibles
                ActualizarListaConyuges(); // Actualizar lista de cónyuges para remover los que ya tienen pareja
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
                mapaService.LimpiarMapaCompleto();
                mapaService.MostrarFamiliaresEnMapa(sistema.Grafo);
            }
            else
            {
                // Si no hay raíz, solo limpiar
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

        // Método auxiliar para recopilar un nodo y todos sus descendientes
        private void RecopilarNodoYDescendientes(NodoFamiliar nodo, ListaEnlazada<NodoFamiliar> lista)
        {
            if (nodo == null || lista.Contiene(nodo))
                return;
                
            lista.AgregarFinal(nodo);
            
            // Recopilar todos los hijos recursivamente
            for (int i = 0; i < nodo.Hijos.Largo(); i++)
            {
                RecopilarNodoYDescendientes(nodo.Hijos.Obtener(i), lista);
            }
        }

        // Botón limpiar campos
        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
        }

        // Botón eliminar miembro con descendencia
        private void EliminarConDescendencia_Click(object sender, RoutedEventArgs e)
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
                // Recopilar todos los nodos a eliminar (el nodo y sus descendientes)
                var nodosAEliminar = new ListaEnlazada<NodoFamiliar>();
                RecopilarNodoYDescendientes(miembro, nodosAEliminar);
                
                // Eliminar referencias de padres en el nodo a eliminar
                for (int i = 0; i < miembro.Padres.Largo(); i++)
                {
                    var padre = miembro.Padres.Obtener(i);
                    // Buscar y eliminar este nodo de la lista de hijos del padre
                    for (int j = 0; j < padre.Hijos.Largo(); j++)
                    {
                        if (padre.Hijos.Obtener(j).Cedula == miembro.Cedula)
                        {
                            padre.Hijos.EliminarPorIndice(j);
                            break;
                        }
                    }
                }
                
                // Si el miembro tiene cónyuge, eliminar la relación bidireccional
                if (miembro.Conyuge != null)
                {
                    miembro.Conyuge.Conyuge = null;
                }

                // Eliminar el miembro del árbol
                exito = sistema.Arbol.EliminarMiembro(nombreEliminar);
                
                // Eliminar todos los nodos del grafo
                if (exito)
                {
                    for (int i = 0; i < nodosAEliminar.Largo(); i++)
                    {
                        var nodo = nodosAEliminar.Obtener(i);
                        sistema.Grafo.EliminarNodo(nodo.Cedula);
                    }
                    sistema.Grafo.RecalcularTodasDistancias();
                }
            }

            if (exito)
            {
                string mensaje = esConyuge 
                    ? $"'{nombreEliminar}' (cónyuge) ha sido eliminado." 
                    : $"'{nombreEliminar}' y sus descendientes han sido eliminados del árbol.";
                    
                MessageBox.Show(mensaje, "Éxito", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                
                ActualizarListaPadres();
                ActualizarListaConyuges();
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

        // Botón marcar como desconocido
        private void MarcarDesconocido_Click(object sender, RoutedEventArgs e)
        {
            string nombreEliminar = cmbEliminar.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreEliminar) || nombreEliminar == "(Sin miembros en el árbol)")
            {
                MessageBox.Show("Por favor, selecciona un miembro para marcar como desconocido.", 
                    "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Confirmar acción
            var result = MessageBox.Show($"¿Estás seguro de que deseas marcar a '{nombreEliminar}' como 'Desconocido'?\n" +
                "Este nodo mantendrá su descendencia y podrá ser editado después.", 
                "Confirmar Acción", MessageBoxButton.YesNo, MessageBoxImage.Question);

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

            // Calcular edad mínima basada en hijos si tiene
            int edadMinima = 0;
            if (miembro.Hijos.Largo() > 0)
            {
                for (int i = 0; i < miembro.Hijos.Largo(); i++)
                {
                    var hijo = miembro.Hijos.Obtener(i);
                    int edadNecesaria = hijo.Edad + 10; // Mínimo 10 años de diferencia
                    if (edadNecesaria > edadMinima)
                        edadMinima = edadNecesaria;
                }
            }

            // Convertir el nodo en "Desconocido"
            miembro.Nombre = "Desconocido";
            miembro.Cedula = "000";
            
            // Si tiene hijos, calcular fecha/edad apropiada
            if (edadMinima > 0)
            {
                miembro.Edad = edadMinima;
                DateTime fechaAproximada = DateTime.Now.AddYears(-edadMinima);
                miembro.FechaNacimiento = fechaAproximada.ToString("dd/MM/yyyy");
            }
            else
            {
                miembro.FechaNacimiento = "01/01/1900";
                miembro.Edad = DateTime.Now.Year - 1900;
            }
            
            miembro.FotoRuta = null;
            miembro.Latitud = double.NaN;
            miembro.Longitud = double.NaN;

            MessageBox.Show($"'{nombreEliminar}' ha sido marcado como 'Desconocido'.\n" +
                "Puedes hacer clic en el nodo para editar su información.", 
                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            ActualizarListaPadres();
            arbolView.DibujarArbol(sistema.Arbol);
            ActualizarMapa();
            ActualizarEstadisticas_Click(null, null);
            cmbEliminar.SelectedIndex = -1;
            cmbEliminar.Text = "";
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
            cmbHermano.SelectedIndex = -1;
            cmbHermano.Text = "";
        }

        // Botón para agregar padres a un miembro existente
        private void AgregarPadresAMiembro_Click(object sender, RoutedEventArgs e)
        {
            // Validar campos obligatorios del nuevo padre/madre
            string nombre = txtNombre.Text.Trim();
            string cedula = txtCedula.Text.Trim();
            string fechaNacimiento = txtFechaNacimiento.Text.Trim();
            string edadTexto = txtEdad.Text.Trim();
            string latitudTexto = txtLatitud.Text.Trim();
            string longitudTexto = txtLongitud.Text.Trim();
            string fotoRuta = txtFotoRuta.Text.Trim();

            // Validar que se haya seleccionado un miembro
            string miembroSeleccionado = cmbMiembroParaPadres.Text.Trim();
            if (string.IsNullOrWhiteSpace(miembroSeleccionado) || miembroSeleccionado.StartsWith("("))
            {
                MessageBox.Show("Por favor, selecciona un miembro al que agregarle padres.",
                    "Selección Requerida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Extraer el nombre del miembro (antes del paréntesis si lo tiene)
            string nombreMiembro = miembroSeleccionado.Split('(')[0].Trim();

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

            if (!ValidacionHelper.ValidarEdad(fechaNac, edad, out _))
            {
                int edadCalculada = DateTime.Now.Year - fechaNac.Year;
                if (fechaNac.Date > DateTime.Now.AddYears(-edadCalculada))
                    edadCalculada--;

                MessageBox.Show($"La edad ingresada ({edad}) no coincide con la fecha de nacimiento ({fechaNacimiento}).\n" +
                    $"La edad correcta basada en tu fecha de nacimiento es {edadCalculada} años.",
                    "Edad Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidacionHelper.ValidarCoordenadas(latitudTexto, longitudTexto, out double latitud, out double longitud, out string mensajeCoordenadas))
            {
                MessageBox.Show(mensajeCoordenadas, "Coordenada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Buscar el miembro al que se le agregarán padres
            var miembro = sistema.Arbol.BuscarPorNombre(nombreMiembro);
            if (miembro == null)
            {
                MessageBox.Show($"No se encontró el miembro '{nombreMiembro}'.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validar que el padre sea mayor que el hijo
            if (miembro.Edad >= edad)
            {
                MessageBox.Show($"El padre/madre debe ser mayor que el hijo.\n" +
                    $"Edad del hijo '{miembro.Nombre}': {miembro.Edad} años\n" +
                    $"Edad del padre/madre: {edad} años",
                    "Edad Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar fechas de nacimiento
            if (!string.IsNullOrEmpty(miembro.FechaNacimiento) &&
                DateTime.TryParseExact(miembro.FechaNacimiento, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime fechaNacHijo))
            {
                if (fechaNac >= fechaNacHijo)
                {
                    MessageBox.Show($"El padre/madre debe nacer antes que el hijo.\n" +
                        $"Fecha de nacimiento del hijo: {miembro.FechaNacimiento}\n" +
                        $"Fecha de nacimiento del padre/madre: {fechaNacimiento}",
                        "Fecha Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validar diferencia de edad mínima
                if (!ValidacionHelper.ValidarDiferenciaEdadPadreHijo(fechaNac, fechaNacHijo, out string mensajeDiferencia))
                {
                    MessageBox.Show(mensajeDiferencia, "Diferencia de Edad Inválida",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Crear el nuevo padre/madre
            NodoFamiliar nuevoPadre = new NodoFamiliar(nombre, cedula, fechaNacimiento, edad, fotoRuta, latitud, longitud);

            // Agregar el padre
            bool exito = sistema.Arbol.AgregarPadreAMiembro(nombreMiembro, nuevoPadre);
            
            string mensaje = "";
            if (exito)
            {
                // Agregar al grafo
                var nodoGrafo = new GrafoGeografico.NodoGrafo(nuevoPadre.Cedula, nuevoPadre.Nombre, nuevoPadre.Latitud, nuevoPadre.Longitud, nuevoPadre.FotoRuta);
                sistema.Grafo.AgregarNodo(nodoGrafo);
                sistema.Grafo.RecalcularTodasDistancias();
                
                mensaje = $"'{nuevoPadre.Nombre}' ha sido agregado como padre/madre de '{nombreMiembro}'.";
                
                if (miembro.Padres.Largo() == 2)
                {
                    var otroPadre = miembro.Padres.Obtener(0).Nombre == nuevoPadre.Nombre 
                        ? miembro.Padres.Obtener(1).Nombre 
                        : miembro.Padres.Obtener(0).Nombre;
                    mensaje += $"\n\n'{nombreMiembro}' ahora tiene 2 padres: {nuevoPadre.Nombre} y {otroPadre}.";
                }
                else if (miembro.Padres.Largo() == 1)
                {
                    mensaje += $"\n\nPuedes agregar el segundo padre repitiendo este proceso.";
                }
            }

            if (exito)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                
                ActualizarListaPadres();
                ActualizarListaConyuges(); // Actualizar para remover los que ya tienen pareja
                ActualizarListaMiembrosParaPadres();
                arbolView.DibujarArbol(sistema.Arbol);
                ActualizarMapa();
                ActualizarEstadisticas_Click(null, null);
                LimpiarCampos();
                cmbMiembroParaPadres.SelectedIndex = -1;
                cmbMiembroParaPadres.Text = "";
            }
            else
            {
                MessageBox.Show($"No se pudo agregar el padre/madre a '{nombreMiembro}'.\n" +
                    "Verifica que el miembro no tenga ya 2 padres.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

