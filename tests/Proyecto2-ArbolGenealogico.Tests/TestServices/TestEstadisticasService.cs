using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;
using Proyecto2_ArbolGenealogico.Services;

namespace Proyecto2_ArbolGenealogico.Tests.Services
{
    public class TestEstadisticasService
    {
        [Fact]
        public void CalcularEstadisticas_SinRaiz_RetornaError()
        {
            // Arrange
            var arbol = new ArbolGenealogico();

            // Act
            var resultado = EstadisticasService.CalcularEstadisticas(arbol);

            // Assert
            Assert.False(resultado.HayDatosSuficientes);
            Assert.Equal("No hay familiares suficientes.", resultado.MensajeError);
        }

        [Fact]
        public void CalcularEstadisticas_ConUnSoloMiembro_RetornaError()
        {
            // Arrange
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "123", "01/01/1980", 44, "foto.jpg", 9.93, -84.08);
            arbol.CrearRaiz(raiz);

            // Act
            var resultado = EstadisticasService.CalcularEstadisticas(arbol);

            // Assert
            Assert.False(resultado.HayDatosSuficientes);
            Assert.Equal("Se necesitan al menos 2 familiares.", resultado.MensajeError);
        }

        [Fact]
        public void CalcularEstadisticas_ConDosMiembros_CalculaCorrectamente()
        {
            // Arrange
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("SanJose", "111", "01/01/1980", 44, "f1.jpg", 9.93, -84.08);
            var hijo = new NodoFamiliar("Cartago", "222", "01/01/2005", 19, "f2.jpg", 9.86, -83.92);

            arbol.CrearRaiz(raiz);
            arbol.AgregarMiembro("SanJose", hijo);

            // Act
            var resultado = EstadisticasService.CalcularEstadisticas(arbol);

            // Assert
            Assert.True(resultado.HayDatosSuficientes);
            Assert.Equal(1, resultado.TotalPares);
            Assert.True(resultado.DistanciaMaxima > 0);
            Assert.Equal(resultado.DistanciaMaxima, resultado.DistanciaMinima); // Solo hay 1 par
            Assert.Equal(resultado.DistanciaMaxima, resultado.DistanciaPromedio);
        }

        [Fact]
        public void CalcularEstadisticas_ConTresMiembros_IdentificaParMasLejanoYCercano()
        {
            // Arrange
            var arbol = new ArbolGenealogico();
            var nodo1 = new NodoFamiliar("Cerca1", "111", "01/01/1970", 54, "f1.jpg", 9.93, -84.08);
            var nodo2 = new NodoFamiliar("Cerca2", "222", "01/01/1995", 29, "f2.jpg", 9.94, -84.09); // Muy cerca de nodo1
            var nodo3 = new NodoFamiliar("Lejos", "333", "01/01/2000", 24, "f3.jpg", 10.63, -85.44); // Lejos de ambos

            arbol.CrearRaiz(nodo1);
            arbol.AgregarMiembro("Cerca1", nodo2);
            arbol.AgregarMiembro("Cerca1", nodo3);

            // Act
            var resultado = EstadisticasService.CalcularEstadisticas(arbol);

            // Assert
            Assert.True(resultado.HayDatosSuficientes);
            Assert.Equal(3, resultado.TotalPares); // 3 combinaciones: 1-2, 1-3, 2-3

            // Par más cercano debe ser Cerca1 y Cerca2
            Assert.True(
                (resultado.ParCercanoA.Nombre == "Cerca1" && resultado.ParCercanoB.Nombre == "Cerca2") ||
                (resultado.ParCercanoA.Nombre == "Cerca2" && resultado.ParCercanoB.Nombre == "Cerca1")
            );

            // Distancia mínima debe ser menor que la máxima
            Assert.True(resultado.DistanciaMinima < resultado.DistanciaMaxima);
        }

        [Fact]
        public void CalcularEstadisticas_IgnoraNodosConCoordenadasInvalidas()
        {
            // Arrange
            var arbol = new ArbolGenealogico();
            var nodo1 = new NodoFamiliar("Valido1", "111", "01/01/1980", 44, "f1.jpg", 9.93, -84.08);
            var nodo2 = new NodoFamiliar("Desconocido", "222", "01/01/1950", 74, null, double.NaN, double.NaN);
            var nodo3 = new NodoFamiliar("Valido2", "333", "01/01/2000", 24, "f3.jpg", 10.63, -85.44);

            arbol.CrearRaiz(nodo1);
            arbol.AgregarMiembro("Valido1", nodo2);
            arbol.AgregarMiembro("Valido1", nodo3);

            // Act
            var resultado = EstadisticasService.CalcularEstadisticas(arbol);

            // Assert
            Assert.True(resultado.HayDatosSuficientes);
            Assert.Equal(1, resultado.TotalPares); // Solo el par Valido1-Valido2
            Assert.Equal("Valido1", resultado.ParLejanoA.Nombre);
            Assert.Equal("Valido2", resultado.ParLejanoB.Nombre);
        }

        [Fact]
        public void CalcularEstadisticas_TodosConCoordenadasInvalidas_RetornaError()
        {
            // Arrange
            var arbol = new ArbolGenealogico();
            var nodo1 = new NodoFamiliar("Desc1", "111", "01/01/1980", 44, null, double.NaN, double.NaN);
            var nodo2 = new NodoFamiliar("Desc2", "222", "01/01/1950", 74, null, double.NaN, double.NaN);

            arbol.CrearRaiz(nodo1);
            arbol.AgregarMiembro("Desc1", nodo2);

            // Act
            var resultado = EstadisticasService.CalcularEstadisticas(arbol);

            // Assert
            Assert.False(resultado.HayDatosSuficientes);
            Assert.Equal("No hay suficientes familiares con coordenadas válidas.", resultado.MensajeError);
            Assert.Equal(0, resultado.DistanciaPromedio);
        }

        [Fact]
        public void CalcularEstadisticas_ConCincoMiembros_CalculaPromedioCorrectamente()
        {
            // Arrange
            var arbol = new ArbolGenealogico();
            arbol.CrearRaiz(new NodoFamiliar("N1", "1", "01/01/1950", 74, "f1.jpg", 9.93, -84.08));
            arbol.AgregarMiembro("N1", new NodoFamiliar("N2", "2", "01/01/1975", 49, "f2.jpg", 10.0, -84.5));
            arbol.AgregarMiembro("N1", new NodoFamiliar("N3", "3", "01/01/1978", 46, "f3.jpg", 9.5, -83.5));
            arbol.AgregarMiembro("N2", new NodoFamiliar("N4", "4", "01/01/2000", 24, "f4.jpg", 10.5, -85.0));
            arbol.AgregarMiembro("N3", new NodoFamiliar("N5", "5", "01/01/2003", 21, "f5.jpg", 9.0, -83.0));

            // Act
            var resultado = EstadisticasService.CalcularEstadisticas(arbol);

            // Assert
            Assert.True(resultado.HayDatosSuficientes);
            Assert.Equal(10, resultado.TotalPares); // Combinaciones de 5: C(5,2) = 10
            Assert.True(resultado.DistanciaPromedio > 0);
            Assert.True(resultado.DistanciaPromedio <= resultado.DistanciaMaxima);
            Assert.True(resultado.DistanciaPromedio >= resultado.DistanciaMinima);
        }
    }
}