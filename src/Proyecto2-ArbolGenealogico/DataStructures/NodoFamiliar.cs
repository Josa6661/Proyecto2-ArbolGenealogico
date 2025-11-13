using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    // Nodo para el árbol genealógico
    public class NodoFamiliar
    {
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public string FotoRuta { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public NodoFamiliar Padre { get; set; }
        public NodoFamiliar Conyuge { get; set; }
        public ListaEnlazada<NodoFamiliar> Hijos { get; set; }

        public NodoFamiliar(string nombre, string cedula, string fechaNacimiento, int edad, string fotoRuta, double latitud, double longitud)
        {
            Nombre = nombre;
            Cedula = cedula;
            FechaNacimiento = fechaNacimiento;
            Edad = edad;
            FotoRuta = fotoRuta;
            Latitud = latitud;
            Longitud = longitud;
            Padre = null;
            Conyuge = null;
            Hijos = new ListaEnlazada<NodoFamiliar>();
        }

        // Agrega hijo y asigna el padre
        public void AgregarHijo(NodoFamiliar hijo)
        {
            Hijos.AgregarFinal(hijo);
            hijo.Padre = this;
        }

        // Establece la relación de cónyuge (bidireccional)
        public void EstablecerConyuge(NodoFamiliar conyuge)
        {
            this.Conyuge = conyuge;
            conyuge.Conyuge = this;
        }
    }
}
