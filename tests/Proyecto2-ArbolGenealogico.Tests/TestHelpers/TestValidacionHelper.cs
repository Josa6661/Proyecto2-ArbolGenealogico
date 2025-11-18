// tests/Proyecto2-ArbolGenealogico.Tests/Helpers/TestValidacionHelper.cs
using System;
using Xunit;
using Proyecto2_ArbolGenealogico.Helpers;

namespace Proyecto2_ArbolGenealogico.Tests.Helpers
{
    public class TestValidacionHelper
    {
        // ========== TESTS DE VALIDACIÓN DE FECHA ==========

        [Fact]
        public void ValidarFecha_ConFormatoValido_RetornaTrue()
        {
            // Arrange
            string fecha = "15/03/1990";

            // Act
            bool resultado = ValidacionHelper.ValidarFecha(fecha, out DateTime fechaParseada);

            // Assert
            Assert.True(resultado);
            Assert.Equal(new DateTime(1990, 3, 15), fechaParseada);
        }

        [Fact]
        public void ValidarFecha_ConFormatoInvalido_RetornaFalse()
        {
            // Arrange
            string fecha = "1990-03-15"; // Formato incorrecto

            // Act
            bool resultado = ValidacionHelper.ValidarFecha(fecha, out DateTime fechaParseada);

            // Assert
            Assert.False(resultado);
        }

        [Theory]
        [InlineData("32/01/2020")] // Día inválido
        [InlineData("15/13/2020")] // Mes inválido
        [InlineData("15-03-2020")] // Separador incorrecto
        [InlineData("15/3/2020")]  // Mes sin cero
        [InlineData("abc/def/ghij")] // No numérico
        public void ValidarFecha_ConFechasInvalidas_RetornaFalse(string fecha)
        {
            bool resultado = ValidacionHelper.ValidarFecha(fecha, out _);
            Assert.False(resultado);
        }

        // ========== TESTS DE VALIDACIÓN DE EDAD ==========

        [Fact]
        public void ValidarEdad_EdadCoincideConFecha_RetornaTrue()
        {
            // Arrange
            DateTime fechaNacimiento = new DateTime(2000, 1, 1);
            int edadIngresada = DateTime.Now.Year - 2000;
            if (DateTime.Now < new DateTime(DateTime.Now.Year, 1, 1))
                edadIngresada--;

            // Act
            bool resultado = ValidacionHelper.ValidarEdad(fechaNacimiento, edadIngresada, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarEdad_EdadNoCoincide_RetornaFalseConMensaje()
        {
            // Arrange
            DateTime fechaNacimiento = new DateTime(2000, 1, 1);
            int edadIncorrecta = 30; // Edad que no coincide

            // Act
            bool resultado = ValidacionHelper.ValidarEdad(fechaNacimiento, edadIncorrecta, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
            Assert.Contains("no coincide", mensaje);
        }

        // ========== TESTS DE VALIDACIÓN DE COORDENADAS ==========

        [Fact]
        public void ValidarCoordenadas_ConCoordenadasValidas_RetornaTrue()
        {
            // Arrange
            string latitud = "9.9281";
            string longitud = "-84.0907";

            // Act
            bool resultado = ValidacionHelper.ValidarCoordenadas(
                latitud, longitud,
                out double lat, out double lon,
                out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Equal(9.9281, lat);
            Assert.Equal(-84.0907, lon);
            Assert.Empty(mensaje);
        }

        [Theory]
        [InlineData("91.0", "0.0")]    // Latitud > 90
        [InlineData("-91.0", "0.0")]   // Latitud < -90
        [InlineData("0.0", "181.0")]   // Longitud > 180
        [InlineData("0.0", "-181.0")]  // Longitud < -180
        public void ValidarCoordenadas_FueraDeRango_RetornaFalse(string lat, string lon)
        {
            bool resultado = ValidacionHelper.ValidarCoordenadas(
                lat, lon, out _, out _, out string mensaje);

            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        [Theory]
        [InlineData("abc", "0.0")]
        [InlineData("0.0", "xyz")]
        [InlineData("", "0.0")]
        [InlineData("0.0", "")]
        public void ValidarCoordenadas_ConFormatoInvalido_RetornaFalse(string lat, string lon)
        {
            bool resultado = ValidacionHelper.ValidarCoordenadas(
                lat, lon, out _, out _, out string mensaje);

            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        // ========== TESTS DE VALIDACIÓN DE CÉDULA ==========

        [Fact]
        public void ValidarCedula_ConSoloNumeros_RetornaTrue()
        {
            // Arrange
            string cedula = "123456789";

            // Act
            bool resultado = ValidacionHelper.ValidarCedula(cedula, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Theory]
        [InlineData("12345ABC")]
        [InlineData("123-456-789")]
        [InlineData("123 456 789")]
        [InlineData("")]
        [InlineData("   ")]
        public void ValidarCedula_ConCaracteresInvalidos_RetornaFalse(string cedula)
        {
            bool resultado = ValidacionHelper.ValidarCedula(cedula, out string mensaje);

            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        // ========== TESTS DE DIFERENCIA DE EDAD PADRE-HIJO ==========

        [Fact]
        public void ValidarDiferenciaEdadPadreHijo_ConDiferenciaValida_RetornaTrue()
        {
            // Arrange
            DateTime fechaPadre = new DateTime(1970, 1, 1);
            DateTime fechaHijo = new DateTime(1990, 1, 1); // 20 años después

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadPadreHijo(
                fechaPadre, fechaHijo, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarDiferenciaEdadPadreHijo_ConMenosDe10Anios_RetornaFalse()
        {
            // Arrange
            DateTime fechaPadre = new DateTime(1990, 1, 1);
            DateTime fechaHijo = new DateTime(1995, 1, 1); // Solo 5 años

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadPadreHijo(
                fechaPadre, fechaHijo, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("10 años", mensaje);
        }

        [Fact]
        public void ValidarDiferenciaEdadPadreHijo_ConExactamente10Anios_RetornaTrue()
        {
            // Arrange
            DateTime fechaPadre = new DateTime(1980, 1, 1);
            DateTime fechaHijo = new DateTime(1990, 1, 1); // Exactamente 10 años

            // Act
            bool resultado = ValidacionHelper.ValidarDiferenciaEdadPadreHijo(
                fechaPadre, fechaHijo, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        // ========== TESTS DE VALIDACIÓN DE CAMPOS REQUERIDOS ==========

        [Fact]
        public void ValidarCamposRequeridos_ConTodosCamposCompletos_RetornaTrue()
        {
            // Arrange
            string nombre = "Juan Pérez";
            string cedula = "123456789";
            string fechaNacimiento = "01/01/1990";
            string edad = "34";
            string latitud = "9.93";
            string longitud = "-84.08";

            // Act
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                nombre, cedula, fechaNacimiento, edad, latitud, longitud,
                out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Theory]
        [InlineData("", "123", "01/01/1990", "30", "9.93", "-84.08")] // Nombre vacío
        [InlineData("Juan", "", "01/01/1990", "30", "9.93", "-84.08")] // Cédula vacía
        [InlineData("Juan", "123", "", "30", "9.93", "-84.08")] // Fecha vacía
        [InlineData("Juan", "123", "01/01/1990", "", "9.93", "-84.08")] // Edad vacía
        [InlineData("Juan", "123", "01/01/1990", "30", "", "-84.08")] // Latitud vacía
        [InlineData("Juan", "123", "01/01/1990", "30", "9.93", "")] // Longitud vacía
        public void ValidarCamposRequeridos_ConCampoVacio_RetornaFalse(
            string nombre, string cedula, string fecha, string edad, string lat, string lon)
        {
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                nombre, cedula, fecha, edad, lat, lon, out string mensaje);

            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        [Theory]
        [InlineData("   ", "123", "01/01/1990", "30", "9.93", "-84.08")] // Solo espacios
        [InlineData(null, "123", "01/01/1990", "30", "9.93", "-84.08")] // Null
        public void ValidarCamposRequeridos_ConCampoNuloOEspacios_RetornaFalse(
            string nombre, string cedula, string fecha, string edad, string lat, string lon)
        {
            bool resultado = ValidacionHelper.ValidarCamposRequeridos(
                nombre, cedula, fecha, edad, lat, lon, out string mensaje);

            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }
    }
}