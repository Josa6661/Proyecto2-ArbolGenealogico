using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestCola
    {
        [Fact]
        public void Test_Cola_CrearCola_Vacia()
        {
            // Arrange
            var cola = new Cola<int>();
            // Act
            bool estaVacia = cola.EstaVacia();
            // Assert
            Assert.True(estaVacia);
            Assert.Equal(0, cola.Largo());
        }

        [Fact]
        public void Test_Cola_Encolar_Elementos()
        {
            // Arrange
            var cola = new Cola<int>();
            // Act
            cola.Encolar(10);
            cola.Encolar(20);
            cola.Encolar(30);
            // Assert
            Assert.False(cola.EstaVacia());
            Assert.Equal(3, cola.Largo());
            Assert.Equal(10, cola.VerFrente());
        }

        [Fact]
        public void Test_Cola_VerFrente_RetornaElementoFrontal()
        {
            // Arrange
            var cola = new Cola<int>();
            cola.Encolar(10);
            cola.Encolar(20);
            cola.Encolar(30);
            // Act
            int frente = cola.VerFrente();
            // Assert
            Assert.Equal(10, frente);
            Assert.Equal(3, cola.Largo()); // Verificar que no se eliminó el elemento
        }

        [Fact]
        public void Test_Cola_Desencolar_EliminaYRetornaElementoFrontal()
        {
            // Arrange
            var cola = new Cola<int>();
            cola.Encolar(100);
            cola.Encolar(200);
            cola.Encolar(300);

            // Act
            int elementoDesencolado = cola.Desencolar();

            // Assert
            Assert.Equal(100, elementoDesencolado);
            Assert.Equal(2, cola.Largo());
            Assert.Equal(200, cola.VerFrente());
        }

        [Fact]
        public void Test_Cola_Desencolar_Secuencial()
        {
            // Arrange
            var cola = new Cola<int>();
            cola.Encolar(100);
            cola.Encolar(200);
            cola.Encolar(300);

            // Act & Assert
            Assert.Equal(100, cola.Desencolar());
            Assert.Equal(200, cola.Desencolar());
            Assert.Equal(300, cola.Desencolar());
            Assert.True(cola.EstaVacia());
        }

        [Fact]
        public void Test_Cola_Limpiar_VaciaTodosLosElementos()
        {
            // Arrange
            var cola = new Cola<int>();
            cola.Encolar(100);
            cola.Encolar(200);
            cola.Encolar(300);

            // Act
            cola.Limpiar();

            // Assert
            Assert.True(cola.EstaVacia());
            Assert.Equal(0, cola.Largo());
        }

        [Fact]
        public void Test_Cola_ComportamientoFIFO()
        {
            // Arrange
            var cola = new Cola<int>();
            var elementos = new[] { 1, 2, 3, 4, 5 };

            // Act - Encolar elementos
            foreach (var elemento in elementos)
            {
                cola.Encolar(elemento);
            }

            // Assert - Verificar que salen en el mismo orden (FIFO)
            foreach (var elementoEsperado in elementos)
            {
                Assert.Equal(elementoEsperado, cola.Desencolar());
            }

            Assert.True(cola.EstaVacia());
        }

        [Fact]
        public void Test_Cola_DesencolarColaVacia_LanzaExcepcion()
        {
            // Arrange
            var cola = new Cola<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cola.Desencolar());
        }

        [Fact]
        public void Test_Cola_VerFrenteColaVacia_LanzaExcepcion()
        {
            // Arrange
            var cola = new Cola<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cola.VerFrente());
        }

        [Fact]
        public void Test_Cola_EncolarDesencolarAlternado()
        {
            // Arrange
            var cola = new Cola<int>();

            // Act & Assert
            cola.Encolar(1);
            cola.Encolar(2);
            Assert.Equal(1, cola.Desencolar());

            cola.Encolar(3);
            Assert.Equal(2, cola.Desencolar());
            Assert.Equal(3, cola.Desencolar());

            Assert.True(cola.EstaVacia());
        }

        [Fact]
        public void Test_Cola_ManejaMúltiplesTipos()
        {
            // Test con strings
            var colaString = new Cola<string>();
            colaString.Encolar("Primero");
            colaString.Encolar("Segundo");
            Assert.Equal("Primero", colaString.VerFrente());
            Assert.Equal("Primero", colaString.Desencolar());
            Assert.Equal("Segundo", colaString.VerFrente());

            // Test con objetos personalizados
            var colaPersona = new Cola<Persona>();
            var persona1 = new Persona { Id = 1, Nombre = "Juan" };
            var persona2 = new Persona { Id = 2, Nombre = "María" };

            colaPersona.Encolar(persona1);
            colaPersona.Encolar(persona2);
            Assert.Equal(persona1, colaPersona.Desencolar());
            Assert.Equal(persona2, colaPersona.Desencolar());
        }

        [Fact]
        public void Test_Cola_LargoActualizaCorrectamente()
        {
            // Arrange
            var cola = new Cola<int>();

            // Act & Assert
            Assert.Equal(0, cola.Largo());

            cola.Encolar(1);
            Assert.Equal(1, cola.Largo());

            cola.Encolar(2);
            cola.Encolar(3);
            Assert.Equal(3, cola.Largo());

            cola.Desencolar();
            Assert.Equal(2, cola.Largo());

            cola.Limpiar();
            Assert.Equal(0, cola.Largo());
        }

        [Fact]
        public void Test_Cola_EncolarGranCantidadElementos()
        {
            // Arrange
            var cola = new Cola<int>();
            const int cantidadElementos = 10000;

            // Act
            for (int i = 0; i < cantidadElementos; i++)
            {
                cola.Encolar(i);
            }

            // Assert
            Assert.Equal(cantidadElementos, cola.Largo());
            Assert.Equal(0, cola.VerFrente());

            // Verificar que desencolamos en orden correcto
            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(i, cola.Desencolar());
            }

            Assert.Equal(cantidadElementos - 100, cola.Largo());
        }

        // Clase auxiliar para testing
        private class Persona
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }
    }
}