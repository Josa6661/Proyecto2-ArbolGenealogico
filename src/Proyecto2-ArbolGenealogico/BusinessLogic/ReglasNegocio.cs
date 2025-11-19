using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.BusinessLogic
{
    public static class ReglasNegocio
    {
        public const int DIFERENCIA_EDAD_MINIMA_PADRE_HIJO = 10;

        public static bool ValidarEdadPadreConHijos(NodoFamiliar nodo, int nuevaEdadPadre, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (nodo.Hijos.Largo() == 0)
            {
                return true;
            }

            for (int i = 0; i < nodo.Hijos.Largo(); i++)
            {
                var hijo = nodo.Hijos.Obtener(i);
                int diferenciaEdad = nuevaEdadPadre - hijo.Edad;

                if (diferenciaEdad < DIFERENCIA_EDAD_MINIMA_PADRE_HIJO)
                {
                    mensajeError = $"La edad del padre/madre debe ser al menos {DIFERENCIA_EDAD_MINIMA_PADRE_HIJO} años mayor que la del hijo.\n" +
                                  $"Diferencia actual con {hijo.Nombre}: {diferenciaEdad} años.";
                    return false;
                }
            }

            return true;
        }
    }
}