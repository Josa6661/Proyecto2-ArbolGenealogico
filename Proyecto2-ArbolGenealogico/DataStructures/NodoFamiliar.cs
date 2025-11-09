using System;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    /// <summary>
    /// Nodo para el árbol genealógico
    /// </summary>
    public class NodoFamiliar
    {
        // Propiedades públicas con get/set (en lugar de campos públicos)
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string FotoRuta { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public NodoFamiliar Padre { get; set; }
        public ListaEnlazada<NodoFamiliar> Hijos { get; private set; }

        // Propiedad calculada para la edad
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                int edad = hoy.Year - FechaNacimiento.Year;

                // Ajustar si aún no ha cumplido años este año
                if (FechaNacimiento.Date > hoy.AddYears(-edad))
                    edad--;

                return edad;
            }
        }

        // Constructor
        public NodoFamiliar(string nombre, string cedula, DateTime fechaNacimiento,
                           string fotoRuta, double latitud, double longitud)
        {
            Nombre = nombre;
            Cedula = cedula;
            FechaNacimiento = fechaNacimiento;
            FotoRuta = fotoRuta;
            Latitud = latitud;
            Longitud = longitud;
            Padre = null;
            Hijos = new ListaEnlazada<NodoFamiliar>();
        }

        // Agrega hijo y asigna el padre
        public void AgregarHijo(NodoFamiliar hijo)
        {
            if (hijo == null)
                throw new ArgumentNullException(nameof(hijo), "El hijo no puede ser nulo");

            Hijos.AgregarFinal(hijo);
            hijo.Padre = this;
        }

        /// <summary>
        /// Valida que las coordenadas geográficas estén en rangos válidos
        /// </summary>
        public bool CoordenadasValidas()
        {
            return Latitud >= -90 && Latitud <= 90 &&
                   Longitud >= -180 && Longitud <= 180;
        }

        public override string ToString()
        {
            return $"{Nombre} ({Cedula}) - {Edad} años";
        }
    }
}


// fecha: 9/11/2025 Ricky Wu
// cambie los campos publicos para que sean propiedades con get y set
// en lugar de meter un fecha de nacimiento, agregue una propiedad calculada Edad
// la edad ya no se guarda, se calcula a partir de la fecha de nacimiento
// los hijos como propiedad de solo lectura (solo get) para evitar que se reemplace la lista
// metodo para validar coordenadas geograficas
