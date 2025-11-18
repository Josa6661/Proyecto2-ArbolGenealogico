using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestSistemaFamiliar
    {
        // ========== TESTS DE INICIALIZACIÓN ==========

        [Fact]
        public void Constructor_DebeInicializarArbolYGrafoVacios()
        {
            var sistema = new SistemaFamiliar();

            Assert.NotNull(sistema.Arbol);
            Assert.NotNull(sistema.Grafo);
            Assert.False(sistema.Arbol.TieneRaiz());
            Assert.Equal(0, sistema.Grafo.ObtenerTodosNodos().Largo());
        }

        // ========== TESTS DE AGREGAR MIEMBROS ==========

        [Fact]
        public void AgregarMiembroCompleto_DebeCrearRaizSiNoExiste()
        {
            var sistema = new SistemaFamiliar();
            var raiz = new NodoFamiliar("Abuelo", "123", "01/01/1950", 75, "foto.jpg", 9.93, -84.08);

            bool resultado = sistema.AgregarMiembroCompleto(null, raiz);

            Assert.True(resultado);
            Assert.True(sistema.Arbol.TieneRaiz());
            Assert.Equal(1, sistema.Grafo.ObtenerTodosNodos().Largo());
        }

        [Fact]
        public void AgregarMiembroCompleto_DebeAgregarAArbolYGrafo()
        {
            var sistema = new SistemaFamiliar();
            var raiz = new NodoFamiliar("Padre", "123", "01/01/1970", 55, "foto1.jpg", 9.93, -84.08);
            var hijo = new NodoFamiliar("Hijo", "456", "01/01/2000", 25, "foto2.jpg", 10.63, -85.44);

            sistema.AgregarMiembroCompleto(null, raiz);
            bool resultado = sistema.AgregarMiembroCompleto("Padre", hijo);

            Assert.True(resultado);
            Assert.Equal(2, sistema.Grafo.ObtenerTodosNodos().Largo());
            Assert.NotNull(sistema.Arbol.BuscarPorNombre("Hijo"));
        }

        [Fact]
        public void AgregarMiembroCompleto_DebeRetornarFalsoSiPadreNoExiste()
        {
            var sistema = new SistemaFamiliar();
            var raiz = new NodoFamiliar("Padre", "123", "01/01/1970", 55, "foto1.jpg", 9.93, -84.08);
            var hijo = new NodoFamiliar("Hijo", "456", "01/01/2000", 25, "foto2.jpg", 10.63, -85.44);

            sistema.AgregarMiembroCompleto(null, raiz);
            bool resultado = sistema.AgregarMiembroCompleto("PadreInexistente", hijo);

            Assert.False(resultado);
            Assert.Equal(1, sistema.Grafo.ObtenerTodosNodos().Largo()); // Solo la raíz
        }

        [Fact]
        public void AgregarMiembroCompleto_DebeRecalcularDistanciasAutomaticamente()
        {
            var sistema = new SistemaFamiliar();
            var nodo1 = new NodoFamiliar("Persona1", "123", "01/01/1980", 45, "foto1.jpg", 0.0, 0.0);
            var nodo2 = new NodoFamiliar("Persona2", "456", "01/01/2005", 20, "foto2.jpg", 1.0, 1.0);

            sistema.AgregarMiembroCompleto(null, nodo1);
            sistema.AgregarMiembroCompleto("Persona1", nodo2);

            double distancia = sistema.Grafo.ObtenerDistancia("123", "456");
            Assert.True(distancia > 0);
        }

        [Fact]
        public void AgregarMiembroCompleto_DebeAgregarVariosNiveles()
        {
            var sistema = new SistemaFamiliar();
            var abuelo = new NodoFamiliar("Abuelo", "111", "01/01/1950", 75, "foto1.jpg", 9.93, -84.08);
            var padre = new NodoFamiliar("Padre", "222", "01/01/1975", 50, "foto2.jpg", 10.0, -84.5);
            var hijo = new NodoFamiliar("Hijo", "333", "01/01/2000", 25, "foto3.jpg", 10.5, -85.0);

            sistema.AgregarMiembroCompleto(null, abuelo);
            sistema.AgregarMiembroCompleto("Abuelo", padre);
            sistema.AgregarMiembroCompleto("Padre", hijo);

            Assert.Equal(3, sistema.Grafo.ObtenerTodosNodos().Largo());
            Assert.NotNull(sistema.BuscarPorNombre("Hijo"));
        }

        // ========== TESTS DE BÚSQUEDA ==========

        [Fact]
        public void BuscarPorNombre_DebeEncontrarMiembroExistente()
        {
            var sistema = new SistemaFamiliar();
            var nodo = new NodoFamiliar("Juan", "123", "01/01/1980", 45, "foto.jpg", 9.93, -84.08);

            sistema.AgregarMiembroCompleto(null, nodo);
            var encontrado = sistema.BuscarPorNombre("Juan");

            Assert.NotNull(encontrado);
            Assert.Equal("Juan", encontrado.Nombre);
            Assert.Equal("123", encontrado.Cedula);
        }

        [Fact]
        public void BuscarPorNombre_DebeRetornarNullSiNoExiste()
        {
            var sistema = new SistemaFamiliar();
            var nodo = new NodoFamiliar("Juan", "123", "01/01/1980", 45, "foto.jpg", 9.93, -84.08);

            sistema.AgregarMiembroCompleto(null, nodo);
            var encontrado = sistema.BuscarPorNombre("Pedro");

            Assert.Null(encontrado);
        }

        // ========== TESTS DE ESTADÍSTICAS GEOGRÁFICAS ==========

        [Fact]
        public void ParMasLejano_DebeRetornarParCorrecto()
        {
            var sistema = new SistemaFamiliar();
            var nodo1 = new NodoFamiliar("SanJose", "111", "01/01/1960", 65, "foto1.jpg", 9.93, -84.08);
            var nodo2 = new NodoFamiliar("Cartago", "222", "01/01/1985", 40, "foto2.jpg", 9.86, -83.92);
            var nodo3 = new NodoFamiliar("Liberia", "333", "01/01/2000", 25, "foto3.jpg", 10.63, -85.44);

            sistema.AgregarMiembroCompleto(null, nodo1);
            sistema.AgregarMiembroCompleto("SanJose", nodo2);
            sistema.AgregarMiembroCompleto("SanJose", nodo3);

            var (nombre1, nombre2, distancia) = sistema.ParMasLejano();

            Assert.True(distancia > 0);
            Assert.True(
                (nombre1 == "Cartago" && nombre2 == "Liberia") ||
                (nombre1 == "Liberia" && nombre2 == "Cartago")
            );
        }

        [Fact]
        public void ParMasLejano_DebeRetornarVacioConMenosDe2Nodos()
        {
            var sistema = new SistemaFamiliar();
            var nodo = new NodoFamiliar("Solo", "111", "01/01/1980", 45, "foto.jpg", 9.93, -84.08);

            sistema.AgregarMiembroCompleto(null, nodo);
            var (nombre1, nombre2, distancia) = sistema.ParMasLejano();

            Assert.Equal("", nombre1);
            Assert.Equal("", nombre2);
            Assert.Equal(0, distancia);
        }

        [Fact]
        public void ParMasLejano_DebeRetornarVacioConSistemaVacio()
        {
            var sistema = new SistemaFamiliar();
            var (nombre1, nombre2, distancia) = sistema.ParMasLejano();

            Assert.Equal("", nombre1);
            Assert.Equal("", nombre2);
            Assert.Equal(0, distancia);
        }

        [Fact]
        public void ParMasCercano_DebeRetornarParCorrecto()
        {
            var sistema = new SistemaFamiliar();
            var nodo1 = new NodoFamiliar("Cerca1", "111", "01/01/1970", 55, "foto1.jpg", 9.93, -84.08);
            var nodo2 = new NodoFamiliar("Cerca2", "222", "01/01/1995", 30, "foto2.jpg", 9.94, -84.09);
            var nodo3 = new NodoFamiliar("Lejos", "333", "01/01/2000", 25, "foto3.jpg", 10.63, -85.44);

            sistema.AgregarMiembroCompleto(null, nodo1);
            sistema.AgregarMiembroCompleto("Cerca1", nodo2);
            sistema.AgregarMiembroCompleto("Cerca1", nodo3);

            var (nombre1, nombre2, distancia) = sistema.ParMasCercano();

            Assert.True(distancia > 0);
            Assert.True(
                (nombre1 == "Cerca1" && nombre2 == "Cerca2") ||
                (nombre1 == "Cerca2" && nombre2 == "Cerca1")
            );
        }

        [Fact]
        public void ParMasCercano_DebeRetornarVacioConMenosDe2Nodos()
        {
            var sistema = new SistemaFamiliar();
            var nodo = new NodoFamiliar("Solo", "111", "01/01/1980", 45, "foto.jpg", 9.93, -84.08);

            sistema.AgregarMiembroCompleto(null, nodo);
            var (nombre1, nombre2, distancia) = sistema.ParMasCercano();

            Assert.Equal("", nombre1);
            Assert.Equal("", nombre2);
            Assert.Equal(0, distancia);
        }

        [Fact]
        public void ObtenerDistanciaPromedio_DebeCalcularPromedioCorrectamente()
        {
            var sistema = new SistemaFamiliar();
            var nodo1 = new NodoFamiliar("A", "111", "01/01/1970", 55, "foto1.jpg", 0.0, 0.0);
            var nodo2 = new NodoFamiliar("B", "222", "01/01/1990", 35, "foto2.jpg", 1.0, 0.0);
            var nodo3 = new NodoFamiliar("C", "333", "01/01/2005", 20, "foto3.jpg", 2.0, 0.0);

            sistema.AgregarMiembroCompleto(null, nodo1);
            sistema.AgregarMiembroCompleto("A", nodo2);
            sistema.AgregarMiembroCompleto("A", nodo3);

            double promedio = sistema.ObtenerDistanciaPromedio();

            Assert.True(promedio > 0);
        }

        [Fact]
        public void ObtenerDistanciaPromedio_DebeRetornarCeroConUnSoloNodo()
        {
            var sistema = new SistemaFamiliar();
            var nodo = new NodoFamiliar("Solo", "111", "01/01/1980", 45, "foto.jpg", 9.93, -84.08);

            sistema.AgregarMiembroCompleto(null, nodo);
            double promedio = sistema.ObtenerDistanciaPromedio();

            Assert.Equal(0, promedio);
        }

        // ========== TESTS DE INTEGRACIÓN ==========

        [Fact]
        public void SistemaCompleto_DebeIntegrarArbolYGrafoCorrectamente()
        {
            var sistema = new SistemaFamiliar();

            // Crear familia de 3 generaciones
            var abuelo = new NodoFamiliar("Abuelo", "111", "01/01/1950", 75, "foto1.jpg", 9.93, -84.08);
            var padre = new NodoFamiliar("Padre", "222", "01/01/1975", 50, "foto2.jpg", 9.86, -83.92);
            var hijo = new NodoFamiliar("Hijo", "333", "01/01/2000", 25, "foto3.jpg", 10.63, -85.44);

            sistema.AgregarMiembroCompleto(null, abuelo);
            sistema.AgregarMiembroCompleto("Abuelo", padre);
            sistema.AgregarMiembroCompleto("Padre", hijo);

            // Verificar árbol
            Assert.True(sistema.Arbol.TieneRaiz());
            Assert.NotNull(sistema.BuscarPorNombre("Padre"));
            Assert.NotNull(sistema.BuscarPorNombre("Hijo"));

            // Verificar grafo
            Assert.Equal(3, sistema.Grafo.ObtenerTodosNodos().Largo());
            Assert.True(sistema.Grafo.ObtenerDistancia("111", "222") > 0);

            // Verificar estadísticas
            var (n1, n2, dist) = sistema.ParMasLejano();
            Assert.True(dist > 0);
            Assert.NotEqual("", n1);
            Assert.NotEqual("", n2);
        }

        [Fact]
        public void SistemaCompleto_DebeManejarFamiliaGrande()
        {
            var sistema = new SistemaFamiliar();

            // Agregar 5 miembros
            sistema.AgregarMiembroCompleto(null,
                new NodoFamiliar("Gen1", "1", "01/01/1950", 75, "f1.jpg", 9.93, -84.08));
            sistema.AgregarMiembroCompleto("Gen1",
                new NodoFamiliar("Gen2A", "2", "01/01/1975", 50, "f2.jpg", 10.0, -84.5));
            sistema.AgregarMiembroCompleto("Gen1",
                new NodoFamiliar("Gen2B", "3", "01/01/1978", 47, "f3.jpg", 9.5, -83.5));
            sistema.AgregarMiembroCompleto("Gen2A",
                new NodoFamiliar("Gen3A", "4", "01/01/2000", 25, "f4.jpg", 10.5, -85.0));
            sistema.AgregarMiembroCompleto("Gen2B",
                new NodoFamiliar("Gen3B", "5", "01/01/2003", 22, "f5.jpg", 9.0, -83.0));

            // Verificar estructura
            Assert.Equal(5, sistema.Grafo.ObtenerTodosNodos().Largo());
            Assert.True(sistema.ObtenerDistanciaPromedio() > 0);

            var (_, _, distMax) = sistema.ParMasLejano();
            var (_, _, distMin) = sistema.ParMasCercano();

            Assert.True(distMax >= distMin);
        }
    }
}