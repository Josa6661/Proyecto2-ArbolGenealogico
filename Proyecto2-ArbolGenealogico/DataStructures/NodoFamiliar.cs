using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    // Nodo para el árbol genealógico
    public class NodoFamiliar
    {
        public string Nombre;
        public string Cedula;
        public string FechaNacimiento;
        public int Edad;
        public string FotoRuta;
        public double Latitud;
        public double Longitud;
        public NodoFamiliar? Padre;
        public ListaEnlazada<NodoFamiliar> Hijos;

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
            Hijos = new ListaEnlazada<NodoFamiliar>();
        }

        // Agrega hijo y asigna el padre
        public void AgregarHijo(NodoFamiliar hijo)
        {
            Hijos.AgregarFinal(hijo);
            hijo.Padre = this;
        }
    }
}
