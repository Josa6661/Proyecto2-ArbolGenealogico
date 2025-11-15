using Proyecto2_ArbolGenealogico.DataStructures;
using Proyecto2_ArbolGenealogico.Helpers;
using System;
using System.Windows;

namespace Proyecto2_ArbolGenealogico.Views
{
    public partial class EditarNodoWindow : Window
    {
        private NodoFamiliar nodo;
        public bool DatosModificados { get; private set; }

        public EditarNodoWindow(NodoFamiliar nodo)
        {
            InitializeComponent();
            this.nodo = nodo;
            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombre.Text = nodo.Nombre;
            txtCedula.Text = nodo.Cedula;
            txtFechaNacimiento.Text = nodo.FechaNacimiento;
            txtEdad.Text = nodo.Edad.ToString();
            txtFotoRuta.Text = nodo.FotoRuta ?? "";
            txtLatitud.Text = nodo.Latitud.ToString();
            txtLongitud.Text = nodo.Longitud.ToString();

            // Si es un nodo "Desconocido", permitir editar la cédula
            if (nodo.Cedula.StartsWith("DESCONOCIDO-"))
            {
                txtCedula.IsReadOnly = false;
            }
        }

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

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string cedula = txtCedula.Text.Trim();
            string fechaNacimiento = txtFechaNacimiento.Text.Trim();
            string edadTexto = txtEdad.Text.Trim();
            string latitudTexto = txtLatitud.Text.Trim();
            string longitudTexto = txtLongitud.Text.Trim();

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

            if (!ValidacionHelper.ValidarFecha(fechaNacimiento, out DateTime fechaNac))
            {
                MessageBox.Show("Por favor, ingresa la fecha de nacimiento en formato dd/MM/yyyy (ejemplo: 15/03/1990).", 
                    "Fecha Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show(mensajeEdad, "Edad Inconsistente", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidacionHelper.ValidarCoordenadas(latitudTexto, longitudTexto, out double latitud, out double longitud, out string mensajeCoordenadas))
            {
                MessageBox.Show(mensajeCoordenadas, "Coordenada Inválida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar diferencia de edad con hijos si tiene
            if (nodo.Hijos.Largo() > 0)
            {
                for (int i = 0; i < nodo.Hijos.Largo(); i++)
                {
                    var hijo = nodo.Hijos.Obtener(i);
                    int diferenciaEdad = edad - hijo.Edad;
                    if (diferenciaEdad < 10)
                    {
                        MessageBox.Show($"La edad del padre/madre debe ser al menos 10 años mayor que la del hijo.\n" +
                            $"Diferencia actual con {hijo.Nombre}: {diferenciaEdad} años.", 
                            "Validación de Edad", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            nodo.Nombre = txtNombre.Text.Trim();
            nodo.Cedula = txtCedula.Text.Trim();
            nodo.FechaNacimiento = txtFechaNacimiento.Text.Trim();
            nodo.Edad = edad;
            nodo.FotoRuta = string.IsNullOrWhiteSpace(txtFotoRuta.Text) ? null : txtFotoRuta.Text.Trim();
            nodo.Latitud = latitud;
            nodo.Longitud = longitud;

            DatosModificados = true;
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
