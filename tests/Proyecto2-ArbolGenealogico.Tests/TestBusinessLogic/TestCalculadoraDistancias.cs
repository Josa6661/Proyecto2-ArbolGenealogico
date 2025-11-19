using System;
using Xunit;
using Proyecto2_ArbolGenealogico.BusinessLogic;

namespace Proyecto2_ArbolGenealogico.Tests.BusinessLogic
{
    public class TestCalculadoraDistancias
    {
        [Fact]
        public void CalcularDistanciaHaversine_EntreDosPuntosIdenticos_RetornaCero()
        {
            // Arrange
            double lat = 9.9281;
            double lon = -84.0907;

            // Act
            double distancia = CalculadoraDistancias.CalcularDistanciaHaversine(lat, lon, lat, lon);

            // Assert
            Assert.Equal(0, distancia, 2); // Precisión de 2 decimales
        }

        [Fact]
        public void CalcularDistanciaHaversine_SanJoseACartago_DistanciaAproximada()
        {
            // Arrange - Coordenadas aproximadas
            double latSanJose = 9.9281;
            double lonSanJose = -84.0907;
            double latCartago = 9.8626;
            double lonCartago = -83.9191;

            // Act
            double distancia = CalculadoraDistancias.CalcularDistanciaHaversine(
                latSanJose, lonSanJose, latCartago, lonCartago);

            // Assert - Distancia real ~20 km
            Assert.True(distancia > 15 && distancia < 25,
                $"Distancia esperada entre 15-25 km, obtenida: {distancia}");
        }

        [Fact]
        public void CalcularDistanciaHaversine_EsSimetrica()
        {
            // Arrange
            double lat1 = 10.0;
            double lon1 = -84.0;
            double lat2 = 11.0;
            double lon2 = -85.0;

            // Act
            double distancia1 = CalculadoraDistancias.CalcularDistanciaHaversine(lat1, lon1, lat2, lon2);
            double distancia2 = CalculadoraDistancias.CalcularDistanciaHaversine(lat2, lon2, lat1, lon1);

            // Assert
            Assert.Equal(distancia1, distancia2, 2);
        }

        [Theory]
        [InlineData(0, 0, 0, 90, 10007.543)] // 1/4 del ecuador
        [InlineData(0, 0, 0, 180, 20015.086)] // 1/2 del ecuador
        public void CalcularDistanciaHaversine_DistanciasConocidas_RetornaValorEsperado(
            double lat1, double lon1, double lat2, double lon2, double distanciaEsperada)
        {
            // Act
            double distancia = CalculadoraDistancias.CalcularDistanciaHaversine(lat1, lon1, lat2, lon2);

            // Assert - Con tolerancia del 1%
            double tolerancia = distanciaEsperada * 0.01;
            Assert.True(Math.Abs(distancia - distanciaEsperada) < tolerancia,
                $"Esperado: {distanciaEsperada} ± {tolerancia}, Obtenido: {distancia}");
        }

        [Fact]
        public void CalcularDistanciaHaversine_ConCoordenadasExtremas_NoLanzaExcepcion()
        {
            // Arrange - Polos y antimeridiano
            double latNorte = 90;
            double latSur = -90;
            double lonEste = 180;
            double lonOeste = -180;

            // Act & Assert - No debe lanzar excepciones
            var distancia1 = CalculadoraDistancias.CalcularDistanciaHaversine(latNorte, 0, latSur, 0);
            var distancia2 = CalculadoraDistancias.CalcularDistanciaHaversine(0, lonEste, 0, lonOeste);

            Assert.True(distancia1 > 0);
            Assert.True(distancia2 >= 0);
        }

        [Fact]
        public void CalcularDistanciaHaversine_SiempreRetornaPositivo()
        {
            // Arrange
            Random random = new Random(42); // Semilla fija para reproducibilidad

            // Act & Assert - Probar 100 pares aleatorios
            for (int i = 0; i < 100; i++)
            {
                double lat1 = random.NextDouble() * 180 - 90;
                double lon1 = random.NextDouble() * 360 - 180;
                double lat2 = random.NextDouble() * 180 - 90;
                double lon2 = random.NextDouble() * 360 - 180;

                double distancia = CalculadoraDistancias.CalcularDistanciaHaversine(lat1, lon1, lat2, lon2);

                Assert.True(distancia >= 0,
                    $"Distancia negativa encontrada: {distancia} para ({lat1},{lon1}) -> ({lat2},{lon2})");
            }
        }
    }
}