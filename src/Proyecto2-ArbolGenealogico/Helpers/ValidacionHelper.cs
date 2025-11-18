using System;
using System.Globalization;

namespace Proyecto2_ArbolGenealogico.Helpers
{
    public static class ValidacionHelper
    {
        // Validar que una fecha sea válida en formato dd/MM/yyyy
        public static bool ValidarFecha(string fecha, out DateTime fechaParseada)
        {
            return DateTime.TryParseExact(fecha, "dd/MM/yyyy", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParseada);
        }

        // Validar que la edad calculada desde la fecha de nacimiento sea válida
        public static bool ValidarEdad(DateTime fechaNacimiento, int edadIngresada, out string mensaje)
        {
            int edadCalculada = DateTime.Now.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Now.AddYears(-edadCalculada))
            {
                edadCalculada--;
            }

            if (edadCalculada != edadIngresada)
            {
                mensaje = $"La edad ingresada ({edadIngresada}) no coincide con la calculada desde la fecha de nacimiento ({edadCalculada} años).";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }

        // Validar que las coordenadas sean números válidos
        public static bool ValidarCoordenadas(string latitudStr, string longitudStr, 
            out double latitud, out double longitud, out string mensaje)
        {
            latitud = 0;
            longitud = 0;
            mensaje = string.Empty;

            if (!double.TryParse(latitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out latitud))
            {
                mensaje = "La latitud debe ser un número válido.";
                return false;
            }

            if (!double.TryParse(longitudStr, NumberStyles.Any, CultureInfo.InvariantCulture, out longitud))
            {
                mensaje = "La longitud debe ser un número válido.";
                return false;
            }

            // Validar rangos de coordenadas
            if (latitud < -90 || latitud > 90)
            {
                mensaje = "La latitud debe estar entre -90 y 90.";
                return false;
            }

            if (longitud < -180 || longitud > 180)
            {
                mensaje = "La longitud debe estar entre -180 y 180.";
                return false;
            }

            return true;
        }

        // Validar que la cédula solo contenga números
        public static bool ValidarCedula(string cedula, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                mensaje = "La cédula no puede estar vacía.";
                return false;
            }

            foreach (char c in cedula)
            {
                if (!char.IsDigit(c))
                {
                    mensaje = "La cédula debe contener solo números.";
                    return false;
                }
            }

            mensaje = string.Empty;
            return true;
        }

        // Validar diferencia de edad entre padre e hijo (mínimo 10 años)
        public static bool ValidarDiferenciaEdadPadreHijo(DateTime fechaNacimientoPadre, 
            DateTime fechaNacimientoHijo, out string mensaje)
        {
            var diferenciaAnios = fechaNacimientoHijo.Year - fechaNacimientoPadre.Year;
            
            if (fechaNacimientoHijo < fechaNacimientoPadre.AddYears(diferenciaAnios))
            {
                diferenciaAnios--;
            }

            if (diferenciaAnios < 10)
            {
                mensaje = "El padre debe ser al menos 10 años mayor que el hijo.";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }

        // Validar que todos los campos requeridos estén completos
        public static bool ValidarCamposRequeridos(string nombre, string cedula, string fechaNacimiento,
            string edad, string latitud, string longitud, out string mensaje)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                mensaje = "El nombre completo es requerido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(cedula))
            {
                mensaje = "La cédula es requerida.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fechaNacimiento))
            {
                mensaje = "La fecha de nacimiento es requerida.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(edad))
            {
                mensaje = "La edad es requerida.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(latitud))
            {
                mensaje = "La latitud es requerida.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(longitud))
            {
                mensaje = "La longitud es requerida.";
                return false;
            }

            mensaje = string.Empty;
            return true;


        }
        public static bool ValidarDiferenciaEdadConyuges(int edadMiembro, int edadConyuge, int diferenciaMaxima, out string mensaje)
        {
            int diferencia = Math.Abs(edadMiembro - edadConyuge);

            if (diferencia > diferenciaMaxima)
            {
                mensaje = $"Hay una diferencia de {diferencia} años entre los cónyuges (máximo recomendado: {diferenciaMaxima} años).";
                return false;
            }

            mensaje = string.Empty;
            return true;
        }

    }
}
