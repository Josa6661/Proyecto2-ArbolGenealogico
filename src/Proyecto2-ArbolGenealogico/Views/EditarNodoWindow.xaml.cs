using Proyecto2_ArbolGenealogico.DataStructures;
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
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtFechaNacimiento.Text) ||
                string.IsNullOrWhiteSpace(txtEdad.Text) ||
                string.IsNullOrWhiteSpace(txtLatitud.Text) ||
                string.IsNullOrWhiteSpace(txtLongitud.Text))
            {
                MessageBox.Show("Por favor completa todos los campos obligatorios (*)", 
                    "Campos Requeridos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtEdad.Text, out int edad) || edad < 0)
            {
                MessageBox.Show("La edad debe ser un número válido.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(txtLatitud.Text, out double latitud) || latitud < -90 || latitud > 90)
            {
                MessageBox.Show("La latitud debe estar entre -90 y 90.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(txtLongitud.Text, out double longitud) || longitud < -180 || longitud > 180)
            {
                MessageBox.Show("La longitud debe estar entre -180 y 180.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            nodo.Nombre = txtNombre.Text.Trim();
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
