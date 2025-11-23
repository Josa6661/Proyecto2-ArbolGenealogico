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
        public ListaEnlazada<NodoFamiliar> Padres { get; set; }
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
            Padres = new ListaEnlazada<NodoFamiliar>();
            Conyuge = null;
            Hijos = new ListaEnlazada<NodoFamiliar>();
        }

        // Agrega hijo y asigna este nodo como padre
        public void AgregarHijo(NodoFamiliar hijo)
        {
            Hijos.AgregarFinal(hijo);

            // Verificar si ya es padre (evitar duplicados)
            bool yaEsPadre = false;
            for (int i = 0; i < hijo.Padres.Largo(); i++)
            {
                if (hijo.Padres.Obtener(i) == this)
                {
                    yaEsPadre = true;
                    break;
                }
            }

            // Solo agregar si no es padre y no tiene ya 2 padres
            if (!yaEsPadre && hijo.Padres.Largo() < 2)
            {
                hijo.Padres.AgregarFinal(this);
            }
        }


        // Establece la relación de cónyuge (bidireccional)
        public void EstablecerConyuge(NodoFamiliar conyuge)
        {
            this.Conyuge = conyuge;
            conyuge.Conyuge = this;
        }

        // Agrega un padre (máximo 2)
        public bool AgregarPadre(NodoFamiliar padre)
        {
            if (Padres.Largo() >= 2)
                return false; // Ya tiene 2 padres

            // Verificar que no esté ya en la lista
            for (int i = 0; i < Padres.Largo(); i++)
            {
                if (Padres.Obtener(i) == padre)
                    return false; // Ya es padre
            }

            Padres.AgregarFinal(padre);
            return true;
        }

        // Obtiene el primer padre (o null si no tiene)
        public NodoFamiliar ObtenerPrimerPadre()
        {
            return Padres.Largo() > 0 ? Padres.Obtener(0) : null;
        }

        // Verifica si tiene padre(s)
        public bool TienePadres()
        {
            return Padres.Largo() > 0;
        }

        // Verifica si tiene exactamente 2 padres
        public bool TieneDosPadres()
        {
            return Padres.Largo() == 2;
        }
    }
}
