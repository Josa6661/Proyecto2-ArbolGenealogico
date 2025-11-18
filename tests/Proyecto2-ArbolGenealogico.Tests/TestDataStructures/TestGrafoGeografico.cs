using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestGrafoGeografico
    {
        // ========== TESTS DE CREACIÓN Y AGREGADO DE NODOS ==========

        [Fact]
        public void Constructor_DebeInicializarGrafoVacio()
        {
            var grafo = new GrafoGeografico();
            
            Assert.True(grafo.EstaVacio());
            Assert.Equal(0, grafo.CantidadNodos());
        }

        [Fact]
        public void AgregarNodo_DebeAgregarNodoCorrectamente()
        {
            var grafo = new GrafoGeografico();
            var nodo = new GrafoGeografico.NodoGrafo("123", "Juan", 10.0, -84.0, "foto.jpg");

            grafo.AgregarNodo(nodo);
            var nodos = grafo.ObtenerTodosNodos();

            Assert.Equal(1, nodos.Largo());
            Assert.Equal("123", nodos.Obtener(0).Cedula);
            Assert.Equal("Juan", nodos.Obtener(0).Nombre);
        }

        [Fact]
        public void AgregarNodo_NoDebeAgregarNodoDuplicado()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "Juan", 10.0, -84.0, "foto.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("123", "Pedro", 11.0, -85.0, "foto2.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            
            Assert.Equal(1, grafo.CantidadNodos());
            Assert.Equal("Juan", grafo.ObtenerTodosNodos().Obtener(0).Nombre);
        }

        // ========== TESTS DE DISTANCIAS ==========

        [Fact]
        public void CalcularYGuardarDistancia_DebeCalcularDistanciaCorrectamente()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "San José", 9.9281, -84.0907, "foto1.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("456", "Cartago", 9.8626, -83.9191, "foto2.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            grafo.CalcularYGuardarDistancia("123", "456");

            double distancia = grafo.ObtenerDistancia("123", "456");

            Assert.True(distancia > 0);
            Assert.True(distancia < 50); // Aprox 15-20 km en realidad
        }

        [Fact]
        public void CalcularYGuardarDistancia_DebeSeBidireccional()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "Punto A", 10.0, -84.0, "foto1.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("456", "Punto B", 11.0, -85.0, "foto2.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            grafo.CalcularYGuardarDistancia("123", "456");

            double distancia1 = grafo.ObtenerDistancia("123", "456");
            double distancia2 = grafo.ObtenerDistancia("456", "123");

            Assert.Equal(distancia1, distancia2);
        }

        [Fact]
        public void ObtenerDistancia_DebeRetornarMenosUnoSiNoExiste()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "Punto A", 10.0, -84.0, "foto1.jpg");

            grafo.AgregarNodo(nodo1);
            double distancia = grafo.ObtenerDistancia("123", "999");

            Assert.Equal(-1, distancia);
        }

        [Fact]
        public void RecalcularTodasDistancias_DebeCalcularTodasLasDistancias()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "A", 10.0, -84.0, "foto1.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("456", "B", 11.0, -85.0, "foto2.jpg");
            var nodo3 = new GrafoGeografico.NodoGrafo("789", "C", 12.0, -86.0, "foto3.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            grafo.AgregarNodo(nodo3);
            grafo.RecalcularTodasDistancias();

            Assert.True(grafo.ObtenerDistancia("123", "456") > 0);
            Assert.True(grafo.ObtenerDistancia("123", "789") > 0);
            Assert.True(grafo.ObtenerDistancia("456", "789") > 0);
        }

        // ========== TESTS DE ESTADÍSTICAS ==========

        [Fact]
        public void ObtenerParMasLejano_DebeRetornarParCorrectoCon3Nodos()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto1.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("456", "B", 1.0, 1.0, "foto2.jpg");
            var nodo3 = new GrafoGeografico.NodoGrafo("789", "C", 10.0, 10.0, "foto3.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            grafo.AgregarNodo(nodo3);
            grafo.RecalcularTodasDistancias();

            var resultado = grafo.ObtenerParMasLejano();

            Assert.True(resultado.distancia > 0);
            Assert.True((resultado.cedula1 == "123" && resultado.cedula2 == "789") ||
                       (resultado.cedula1 == "789" && resultado.cedula2 == "123"));
        }

        [Fact]
        public void ObtenerParMasLejano_DebeRetornarVacioConMenosDe2Nodos()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto1.jpg");

            grafo.AgregarNodo(nodo1);
            var resultado = grafo.ObtenerParMasLejano();

            Assert.Equal("", resultado.cedula1);
            Assert.Equal("", resultado.cedula2);
            Assert.Equal(0, resultado.distancia);
        }

        [Fact]
        public void ObtenerParMasCercano_DebeRetornarParCorrectoCon3Nodos()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto1.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("456", "B", 0.01, 0.01, "foto2.jpg");
            var nodo3 = new GrafoGeografico.NodoGrafo("789", "C", 10.0, 10.0, "foto3.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            grafo.AgregarNodo(nodo3);
            grafo.RecalcularTodasDistancias();

            var resultado = grafo.ObtenerParMasCercano();

            Assert.True(resultado.distancia > 0);
            Assert.True((resultado.cedula1 == "123" && resultado.cedula2 == "456") ||
                       (resultado.cedula1 == "456" && resultado.cedula2 == "123"));
        }

        [Fact]
        public void ObtenerParMasCercano_DebeRetornarVacioConMenosDe2Nodos()
        {
            var grafo = new GrafoGeografico();
            var resultado = grafo.ObtenerParMasCercano();

            Assert.Equal("", resultado.cedula1);
            Assert.Equal("", resultado.cedula2);
            Assert.Equal(0, resultado.distancia);
        }

        [Fact]
        public void ObtenerDistanciaPromedio_DebeCalcularPromedioCorrectamente()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto1.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("456", "B", 1.0, 0.0, "foto2.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            grafo.RecalcularTodasDistancias();

            double promedio = grafo.ObtenerDistanciaPromedio();

            Assert.True(promedio > 0);
        }

        [Fact]
        public void ObtenerDistanciaPromedio_DebeRetornarCeroSinDistancias()
        {
            var grafo = new GrafoGeografico();
            double promedio = grafo.ObtenerDistanciaPromedio();

            Assert.Equal(0, promedio);
        }

        // ========== TESTS DE CONSULTAS ==========

        [Fact]
        public void ObtenerDistanciasDesde_DebeRetornarTodasLasDistanciasDesdeUnNodo()
        {
            var grafo = new GrafoGeografico();
            var nodo1 = new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto1.jpg");
            var nodo2 = new GrafoGeografico.NodoGrafo("456", "B", 1.0, 1.0, "foto2.jpg");
            var nodo3 = new GrafoGeografico.NodoGrafo("789", "C", 2.0, 2.0, "foto3.jpg");

            grafo.AgregarNodo(nodo1);
            grafo.AgregarNodo(nodo2);
            grafo.AgregarNodo(nodo3);
            grafo.RecalcularTodasDistancias();

            var distancias = grafo.ObtenerDistanciasDesde("123");

            Assert.Equal(2, distancias.Largo());
            // Verificar que contiene las cédulas correctas
            bool contiene456 = false, contiene789 = false;
            for (int i = 0; i < distancias.Largo(); i++)
            {
                var (cedula, _) = distancias.Obtener(i);
                if (cedula == "456") contiene456 = true;
                if (cedula == "789") contiene789 = true;
            }
            Assert.True(contiene456);
            Assert.True(contiene789);
        }

        [Fact]
        public void ObtenerDistanciasDesde_DebeRetornarVacioSiNodoNoExiste()
        {
            var grafo = new GrafoGeografico();
            var distancias = grafo.ObtenerDistanciasDesde("999");

            Assert.Equal(0, distancias.Largo());
        }

        // ========== TESTS DE NUEVAS FUNCIONALIDADES ==========

        [Fact]
        public void ConstruirDesdeArbol_DebeCargarTodosLosMiembros()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "123", "01/01/1950", 74, "foto1.jpg", 9.93, -84.09);
            var hijo = new NodoFamiliar("Pedro", "456", "01/01/1980", 44, "foto2.jpg", 10.00, -84.00);
            
            arbol.CrearRaiz(raiz);
            raiz.AgregarHijo(hijo);

            var grafo = new GrafoGeografico();
            grafo.ConstruirDesdeArbol(arbol);

            Assert.Equal(2, grafo.CantidadNodos());
            Assert.True(grafo.ObtenerDistancia("123", "456") > 0); // Distancias calculadas
        }

        [Fact]
        public void ConstruirDesdeArbol_DebeIncluirConyuges()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "123", "01/01/1950", 74, "foto1.jpg", 9.93, -84.09);
            var conyuge = new NodoFamiliar("Maria", "456", "01/01/1952", 72, "foto2.jpg", 9.93, -84.09);
            
            arbol.CrearRaiz(raiz);
            raiz.EstablecerConyuge(conyuge);

            var grafo = new GrafoGeografico();
            grafo.ConstruirDesdeArbol(arbol);

            Assert.Equal(2, grafo.CantidadNodos());
        }

        [Fact]
        public void BuscarPorNombre_DebeEncontrarNodoExistente()
        {
            var grafo = new GrafoGeografico();
            var nodo = new GrafoGeografico.NodoGrafo("123", "Juan Pérez", 10.0, -84.0, "foto.jpg");
            grafo.AgregarNodo(nodo);

            var encontrado = grafo.BuscarPorNombre("Juan Pérez");

            Assert.NotNull(encontrado);
            Assert.Equal("123", encontrado.Cedula);
        }

        [Fact]
        public void BuscarPorNombre_DebeRetornarNullSiNoExiste()
        {
            var grafo = new GrafoGeografico();
            var encontrado = grafo.BuscarPorNombre("NoExiste");

            Assert.Null(encontrado);
        }

        [Fact]
        public void RecorridoBFS_DebeRecorrerTodosLosNodos()
        {
            var grafo = new GrafoGeografico();
            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto1.jpg"));
            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("456", "B", 1.0, 1.0, "foto2.jpg"));
            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("789", "C", 2.0, 2.0, "foto3.jpg"));

            var recorrido = grafo.RecorridoBFS("123");

            Assert.Equal(3, recorrido.Largo());
            Assert.Equal("123", recorrido.Obtener(0).Cedula);
        }

        [Fact]
        public void RecorridoBFS_ConNodoInexistente_DebeRetornarListaVacia()
        {
            var grafo = new GrafoGeografico();
            var recorrido = grafo.RecorridoBFS("999");

            Assert.Equal(0, recorrido.Largo());
        }

        [Fact]
        public void RecorridoDFS_DebeRecorrerTodosLosNodos()
        {
            var grafo = new GrafoGeografico();
            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto1.jpg"));
            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("456", "B", 1.0, 1.0, "foto2.jpg"));
            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("789", "C", 2.0, 2.0, "foto3.jpg"));

            var recorrido = grafo.RecorridoDFS("123");

            Assert.Equal(3, recorrido.Largo());
            Assert.Equal("123", recorrido.Obtener(0).Cedula);
        }

        [Fact]
        public void RecorridoDFS_ConNodoInexistente_DebeRetornarListaVacia()
        {
            var grafo = new GrafoGeografico();
            var recorrido = grafo.RecorridoDFS("999");

            Assert.Equal(0, recorrido.Largo());
        }

        [Fact]
        public void CantidadNodos_DebeRetornarCantidadCorrecta()
        {
            var grafo = new GrafoGeografico();
            Assert.Equal(0, grafo.CantidadNodos());

            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto.jpg"));
            Assert.Equal(1, grafo.CantidadNodos());

            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("456", "B", 1.0, 1.0, "foto2.jpg"));
            Assert.Equal(2, grafo.CantidadNodos());
        }

        [Fact]
        public void EstaVacio_DebeRetornarEstadoCorrecto()
        {
            var grafo = new GrafoGeografico();
            Assert.True(grafo.EstaVacio());

            grafo.AgregarNodo(new GrafoGeografico.NodoGrafo("123", "A", 0.0, 0.0, "foto.jpg"));
            Assert.False(grafo.EstaVacio());
        }
    }
}