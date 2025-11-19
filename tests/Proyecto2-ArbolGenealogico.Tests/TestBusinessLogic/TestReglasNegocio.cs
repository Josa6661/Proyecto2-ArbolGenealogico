using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;
using Proyecto2_ArbolGenealogico.BusinessLogic;

namespace Proyecto2_ArbolGenealogico.Tests.BusinessLogic
{
    public class TestReglasNegocio
    {
        [Fact]
        public void ValidarEdadPadreConHijos_SinHijos_RetornaTrue()
        {
            // Arrange
            var padre = new NodoFamiliar("Padre", "123", "01/01/1980", 44, "foto.jpg", 9.93, -84.08);
            int nuevaEdad = 45;

            // Act
            bool resultado = ReglasNegocio.ValidarEdadPadreConHijos(padre, nuevaEdad, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarEdadPadreConHijos_DiferenciaMayorA10_RetornaTrue()
        {
            // Arrange
            var padre = new NodoFamiliar("Padre", "123", "01/01/1980", 44, "foto.jpg", 9.93, -84.08);
            var hijo = new NodoFamiliar("Hijo", "456", "01/01/2005", 19, "foto.jpg", 9.93, -84.08);
            padre.AgregarHijo(hijo);
            int nuevaEdad = 40; // 40 - 19 = 21 años de diferencia

            // Act
            bool resultado = ReglasNegocio.ValidarEdadPadreConHijos(padre, nuevaEdad, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarEdadPadreConHijos_DiferenciaExactamente10_RetornaTrue()
        {
            // Arrange
            var padre = new NodoFamiliar("Padre", "123", "01/01/1980", 44, "foto.jpg", 9.93, -84.08);
            var hijo = new NodoFamiliar("Hijo", "456", "01/01/2005", 19, "foto.jpg", 9.93, -84.08);
            padre.AgregarHijo(hijo);
            int nuevaEdad = 29; // 29 - 19 = 10 años exactos

            // Act
            bool resultado = ReglasNegocio.ValidarEdadPadreConHijos(padre, nuevaEdad, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarEdadPadreConHijos_DiferenciaMenorA10_RetornaFalse()
        {
            // Arrange
            var padre = new NodoFamiliar("Padre", "123", "01/01/1980", 44, "foto.jpg", 9.93, -84.08);
            var hijo = new NodoFamiliar("Hijo", "456", "01/01/2005", 19, "foto.jpg", 9.93, -84.08);
            padre.AgregarHijo(hijo);
            int nuevaEdad = 25; // 25 - 19 = 6 años de diferencia

            // Act
            bool resultado = ReglasNegocio.ValidarEdadPadreConHijos(padre, nuevaEdad, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.Contains("10 años mayor", mensaje);
            Assert.Contains("Hijo", mensaje);
            Assert.Contains("6 años", mensaje);
        }

        [Fact]
        public void ValidarEdadPadreConHijos_VariosHijos_ValidaTodos()
        {
            // Arrange
            var padre = new NodoFamiliar("Padre", "123", "01/01/1970", 54, "foto.jpg", 9.93, -84.08);
            var hijo1 = new NodoFamiliar("Hijo1", "456", "01/01/2000", 24, "foto.jpg", 9.93, -84.08);
            var hijo2 = new NodoFamiliar("Hijo2", "789", "01/01/2005", 19, "foto.jpg", 9.93, -84.08);
            padre.AgregarHijo(hijo1);
            padre.AgregarHijo(hijo2);
            int nuevaEdad = 35; // 35 - 24 = 11 (OK), 35 - 19 = 16 (OK)

            // Act
            bool resultado = ReglasNegocio.ValidarEdadPadreConHijos(padre, nuevaEdad, out string mensaje);

            // Assert
            Assert.True(resultado);
            Assert.Empty(mensaje);
        }

        [Fact]
        public void ValidarEdadPadreConHijos_VariosHijos_FallaConElMenor()
        {
            // Arrange
            var padre = new NodoFamiliar("Padre", "123", "01/01/1970", 54, "foto.jpg", 9.93, -84.08);
            var hijo1 = new NodoFamiliar("Hijo1", "456", "01/01/2000", 24, "foto.jpg", 9.93, -84.08);
            var hijo2 = new NodoFamiliar("Hijo2", "789", "01/01/2005", 19, "foto.jpg", 9.93, -84.08);
            padre.AgregarHijo(hijo1);
            padre.AgregarHijo(hijo2);
            int nuevaEdad = 27; // 27 - 24 = 3 (FALLA), 27 - 19 = 8 (FALLA)

            // Act
            bool resultado = ReglasNegocio.ValidarEdadPadreConHijos(padre, nuevaEdad, out string mensaje);

            // Assert
            Assert.False(resultado);
            Assert.NotEmpty(mensaje);
        }

        [Fact]
        public void ValidarEdadPadreConHijos_ConstanteDiferenciaMinimaEs10()
        {
            // Assert
            Assert.Equal(10, ReglasNegocio.DIFERENCIA_EDAD_MINIMA_PADRE_HIJO);
        }
    }
}