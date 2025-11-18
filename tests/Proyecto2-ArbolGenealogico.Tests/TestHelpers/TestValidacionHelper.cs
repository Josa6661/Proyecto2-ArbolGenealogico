using System;
using Xunit;
using Proyecto2_ArbolGenealogico.Helpers;

namespace Proyecto2_ArbolGenealogico.Tests
{
    public class ValidacionHelperTests
    {
        #region ValidarFecha Tests

        [Theory]
        [InlineData("15/03/1990")]
        [InlineData("01/01/2000")]
        [InlineData("31/12/1985")]
        [InlineData("29/02/2024")] // Año bisiesto
        public void ValidarFecha_FechaValida_RetornaTrue(string fecha)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarFecha(fecha, out DateTime fechaParseada);

            // Assert
            Assert.True(resultado);
            Assert.NotEqual(default(DateTime), fechaParseada);
        }

        [Theory]
        [InlineData("1990-03-15")] // Formato incorrecto
        [InlineData("15/13/1990")] // Mes inválido
        [InlineData("32/12/1990")] // Día inválido
        [InlineData("29/02/2023")] // No es año bisiesto
        [InlineData("abc/def/ghij")] // Texto inválido
        [InlineData("")] // Cadena vacía
        public void ValidarFecha_FechaInvalida_RetornaFalse(string fecha)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarFecha(fecha, out DateTime fechaParseada);

            // Assert
            Assert.False(resultado);
            Assert.Equal(default(DateTime), fechaParseada);
        }

        #endregion

        #region ValidarEdad Tests

        [Fact]
        public void ValidarEdad_EdadCoincideConFechaNacimiento_RetornaTrue()
        {
            // Arrange
            DateTime fechaNacimiento = new DateTime(1990, 1, 1);
            int edadCalculada = DateTime.Now.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Now.AddYears(-edadCalculada))
            {
                edadCalculada--;
            }

            // Act
            bool resultado = ValidacionHelper.ValidarEdad(fechaNacimiento, edadCalculada, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarEdad_EdadNoCoincide_RetornaFalse()
        {
            // Arrange
            DateTime fechaNacimiento = new DateTime(1990, 1, 1);
            int edadIncorrecta = 20; // Edad que no coincide con la fecha

            // Act
            bool resultado = ValidacionHelper.ValidarEdad(fechaNacimiento, edadIncorrecta, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
            Assert.Contains("no coincide", mensaje);
        }

        [Fact]
        public void ValidarEdad_EdadMayorALaReal_RetornaFalse()
        {
            // Arrange
            DateTime fechaNacimiento = DateTime.Now.AddYears(-25);
            int edadIngresada = 30;

            // Act
            bool resultado = ValidacionHelper.ValidarEdad(fechaNacimiento, edadIngresada, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("no coincide", mensaje);
        }

        #endregion

        #region ValidarCoordenadas Tests

        [Theory]
        [InlineData("9.9281", "-84.0907")] // San José, Costa Rica
        [InlineData("0", "0")] // Ecuador y Greenwich
        [InlineData("-90", "-180")] // Límites inferiores
        [InlineData("90", "180")] // Límites superiores
        [InlineData("45.5", "-73.6")] // Con decimales
        public void ValidarCoordenadas_CoordenadasValidas_RetornaTrue(string latitudStr, string longitudStr)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCoordenadas(latitudStr, longitudStr,
                out double latitud, out double longitud, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
            Assert.InRange(latitud, -90, 90);
            Assert.InRange(longitud, -180, 180);
        }

        [Theory]
        [InlineData("abc", "123")] // Latitud no numérica
        [InlineData("45.5", "xyz")] // Longitud no numérica
        [InlineData("", "")] // Vacíos
        public void ValidarCoordenadas_FormatoInvalido_RetornaFalse(string latitudStr, string longitudStr)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCoordenadas(latitudStr, longitudStr,
                out double latitud, out double longitud, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        [Theory]
        [InlineData("91", "0")] // Latitud fuera de rango
        [InlineData("-91", "0")] // Latitud fuera de rango
        [InlineData("0", "181")] // Longitud fuera de rango
        [InlineData("0", "-181")] // Longitud fuera de rango
        public void ValidarCoordenadas_FueraDeRango_RetornaFalse(string latitudStr, string longitudStr)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCoordenadas(latitudStr, longitudStr,
                out double latitud, out double longitud, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
            Assert.Contains("entre", mensaje);
        }

        #endregion

        #region ValidarCedula Tests

        [Theory]
        [InlineData("123456789")]
        [InlineData("0")]
        [InlineData("111111111")]
        [InlineData("987654321")]
        public void ValidarCedula_CedulaNumerica_RetornaTrue(string cedula)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCedula(cedula, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Theory]
        [InlineData("12345ABC")]
        [InlineData("ABC123")]
        [InlineData("123-456-789")]
        [InlineData("123 456 789")]
        [InlineData("123.456.789")]
        public void ValidarCedula_ConCaracteresNoNumericos_RetornaFalse(string cedula)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCedula(cedula, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
            Assert.Contains("solo números", mensaje);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ValidarCedula_CedulaVacia_RetornaFalse(string cedula)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCedula(cedula, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
            Assert.Contains("vacía", mensaje);
        }

        #endregion

        #region ValidarDiferenciaEdadPadreHijo Tests

        [Fact]
        public void ValidarDiferenciaEdadPadreHijo_DiferenciaMayor10Anios_RetornaTrue()
        {
            // Arrange
            DateTime fechaNacimientoPadre = new DateTime(1970, 1, 1);
            DateTime fechaNacimientoHijo = new DateTime(1995, 1, 1); // 25 años de diferencia

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadPadreHijo(
                fechaNacimientoPadre, fechaNacimientoHijo, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarDiferenciaEdadPadreHijo_DiferenciaExactamente10Anios_RetornaTrue()
        {
            // Arrange
            DateTime fechaNacimientoPadre = new DateTime(1970, 6, 15);
            DateTime fechaNacimientoHijo = new DateTime(1980, 6, 15); // Exactamente 10 años

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadPadreHijo(
                fechaNacimientoPadre, fechaNacimientoHijo, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarDiferenciaEdadPadreHijo_DiferenciaMenor10Anios_RetornaFalse()
        {
            // Arrange
            DateTime fechaNacimientoPadre = new DateTime(1990, 1, 1);
            DateTime fechaNacimientoHijo = new DateTime(1995, 1, 1); // 5 años de diferencia

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadPadreHijo(
                fechaNacimientoPadre, fechaNacimientoHijo, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
            Assert.Contains("10 años mayor", mensaje);
        }

        [Fact]
        public void ValidarDiferenciaEdadPadreHijo_HijoMayorQuePadre_RetornaFalse()
        {
            // Arrange
            DateTime fechaNacimientoPadre = new DateTime(1990, 1, 1);
            DateTime fechaNacimientoHijo = new DateTime(1985, 1, 1); // Hijo mayor que padre

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadPadreHijo(
                fechaNacimientoPadre, fechaNacimientoHijo, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        #endregion

        #region ValidarCamposRequeridos Tests

        [Fact]
        public void ValidarCamposRequeridos_TodosLosCamposCompletos_RetornaTrue()
        {
            // Arrange
            string nombre = "Juan Pérez";
            string cedula = "123456789";
            string fechaNacimiento = "15/03/1990";
            string edad = "33";
            string latitud = "9.9281";
            string longitud = "-84.0907";

            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                nombre, cedula, fechaNacimiento, edad, latitud, longitud, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarCamposRequeridos_NombreVacio_RetornaFalse()
        {
            // Arrange
            string nombre = "";

            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                nombre, "123", "01/01/1990", "33", "9.9", "-84.0", out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("nombre", mensaje.ToLower());
        }

        [Fact]
        public void ValidarCamposRequeridos_CedulaVacia_RetornaFalse()
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                "Juan", "", "01/01/1990", "33", "9.9", "-84.0", out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("cédula", mensaje.ToLower());
        }

        [Fact]
        public void ValidarCamposRequeridos_FechaNacimientoVacia_RetornaFalse()
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                "Juan", "123", "", "33", "9.9", "-84.0", out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("fecha de nacimiento", mensaje.ToLower());
        }

        [Fact]
        public void ValidarCamposRequeridos_EdadVacia_RetornaFalse()
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                "Juan", "123", "01/01/1990", "", "9.9", "-84.0", out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("edad", mensaje.ToLower());
        }

        [Fact]
        public void ValidarCamposRequeridos_LatitudVacia_RetornaFalse()
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                "Juan", "123", "01/01/1990", "33", "", "-84.0", out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("latitud", mensaje.ToLower());
        }

        [Fact]
        public void ValidarCamposRequeridos_LongitudVacia_RetornaFalse()
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                "Juan", "123", "01/01/1990", "33", "9.9", "", out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("longitud", mensaje.ToLower());
        }

        [Theory]
        [InlineData(null, "123", "01/01/1990", "33", "9.9", "-84.0")]
        [InlineData("Juan", null, "01/01/1990", "33", "9.9", "-84.0")]
        [InlineData("Juan", "123", null, "33", "9.9", "-84.0")]
        [InlineData("Juan", "123", "01/01/1990", null, "9.9", "-84.0")]
        [InlineData("Juan", "123", "01/01/1990", "33", null, "-84.0")]
        [InlineData("Juan", "123", "01/01/1990", "33", "9.9", null)]
        public void ValidarCamposRequeridos_CampoNull_RetornaFalse(
            string nombre, string cedula, string fechaNacimiento,
            string edad, string latitud, string longitud)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                nombre, cedula, fechaNacimiento, edad, latitud, longitud, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        #endregion

        #region ValidarDiferenciaEdadConyuges Tests

        [Theory]
        [InlineData(30, 32, 10)] // 2 años de diferencia, máximo 10
        [InlineData(40, 40, 10)] // Misma edad
        [InlineData(25, 35, 10)] // Exactamente 10 años
        [InlineData(50, 45, 15)] // 5 años de diferencia, máximo 15
        public void ValidarDiferenciaEdadConyuges_DiferenciaPermitida_RetornaTrue(
            int edadMiembro, int edadConyuge, int diferenciaMaxima)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadConyuges(
                edadMiembro, edadConyuge, diferenciaMaxima, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Theory]
        [InlineData(20, 35, 10)] // 15 años de diferencia, máximo 10
        [InlineData(50, 30, 10)] // 20 años de diferencia, máximo 10
        [InlineData(25, 50, 20)] // 25 años de diferencia, máximo 20
        public void ValidarDiferenciaEdadConyuges_DiferenciaExcedida_RetornaFalse(
            int edadMiembro, int edadConyuge, int diferenciaMaxima)
        {
            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadConyuges(
                edadMiembro, edadConyuge, diferenciaMaxima, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
            Assert.Contains("diferencia", mensaje.ToLower());
            Assert.Contains(diferenciaMaxima.ToString(), mensaje);
        }

        [Fact]
        public void ValidarDiferenciaEdadConyuges_DiferenciaExactamenteMaximo_RetornaTrue()
        {
            // Arrange
            int edadMiembro = 30;
            int edadConyuge = 40;
            int diferenciaMaxima = 10;

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadConyuges(
                edadMiembro, edadConyuge, diferenciaMaxima, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarDiferenciaEdadConyuges_CalculaValorAbsoluto()
        {
            // Act - Probar ambas direcciones
            bool resultado1 = ValidacionHelper.ValidarDiferenciaEdadConyuges(
                30, 40, 10, out string mensaje1);
            bool resultado2 = ValidacionHelper.ValidarDiferenciaEdadConyuges(
                40, 30, 10, out string mensaje2);

            // Assert - Ambos deben dar el mismo resultado
            Assert.Equal(resultado1, resultado2);
            Assert.True(resultado1);
        }

        #endregion
    }
}