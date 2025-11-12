using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestPila
    {
        [Fact]
        public void Test_Pila_CrearPila_Vacia()
        {
            // Arrange
            var pila = new Pila<int>();

            // Act
            bool estaVacia = pila.EstaVacia();

            // Assert
            Assert.True(estaVacia);
            Assert.Equal(0, pila.Largo());
        }

        [Fact]
        public void Test_Pila_Apilar_Elementos()
        {
            // Arrange
            var pila = new Pila<int>();

            // Act
            pila.Apilar(10);
            pila.Apilar(20);
            pila.Apilar(30);

            // Assert
            Assert.False(pila.EstaVacia());
            Assert.Equal(3, pila.Largo());
            Assert.Equal(30, pila.VerTope());
        }

        [Fact]
        public void Test_Pila_VerTope_RetornaElementoSuperior()
        {
            // Arrange
            var pila = new Pila<int>();
            pila.Apilar(10);
            pila.Apilar(20);
            pila.Apilar(30);

            // Act
            int tope = pila.VerTope();

            // Assert
            Assert.Equal(30, tope);
            Assert.Equal(3, pila.Largo()); // Verificar que no se eliminó el elemento
        }

        [Fact]
        public void Test_Pila_Desapilar_EliminaYRetornaElementoSuperior()
        {
            // Arrange
            var pila = new Pila<int>();
            pila.Apilar(10);
            pila.Apilar(20);
            pila.Apilar(30);

            // Act
            int elementoDesapilado = pila.Desapilar();

            // Assert
            Assert.Equal(30, elementoDesapilado);
            Assert.Equal(2, pila.Largo());
            Assert.Equal(20, pila.VerTope());
        }

        [Fact]
        public void Test_Pila_Desapilar_Secuencial()
        {
            // Arrange
            var pila = new Pila<int>();
            pila.Apilar(10);
            pila.Apilar(20);
            pila.Apilar(30);

            // Act & Assert
            Assert.Equal(30, pila.Desapilar());
            Assert.Equal(20, pila.Desapilar());
            Assert.Equal(10, pila.Desapilar());
            Assert.True(pila.EstaVacia());
        }

        [Fact]
        public void Test_Pila_Limpiar_VaciaTodosLosElementos()
        {
            // Arrange
            var pila = new Pila<int>();
            pila.Apilar(10);
            pila.Apilar(20);
            pila.Apilar(30);

            // Act
            pila.Limpiar();

            // Assert
            Assert.True(pila.EstaVacia());
            Assert.Equal(0, pila.Largo());
        }

        [Fact]
        public void Test_Pila_Largo_RetornaCantidadCorrecta()
        {
            // Arrange
            var pila = new Pila<int>();

            // Act & Assert
            Assert.Equal(0, pila.Largo());

            pila.Apilar(10);
            Assert.Equal(1, pila.Largo());

            pila.Apilar(20);
            Assert.Equal(2, pila.Largo());

            pila.Desapilar();
            Assert.Equal(1, pila.Largo());
        }

        [Fact]
        public void Test_Pila_DesapilarPilaVacia_LanzaExcepcion()
        {
            // Arrange
            var pila = new Pila<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => pila.Desapilar());
        }

        [Fact]
        public void Test_Pila_VerTopePilaVacia_LanzaExcepcion()
        {
            // Arrange
            var pila = new Pila<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => pila.VerTope());
        }

        [Fact]
        public void Test_Pila_TrabajaConDiferentesTipos()
        {
            // Test con strings
            var pilaString = new Pila<string>();
            pilaString.Apilar("Hola");
            pilaString.Apilar("Mundo");
            Assert.Equal("Mundo", pilaString.VerTope());
            Assert.Equal(2, pilaString.Largo());

            // Test con objetos personalizados
            var pilaPersona = new Pila<Persona>();
            var persona1 = new Persona { Nombre = "Juan" };
            var persona2 = new Persona { Nombre = "María" };
            pilaPersona.Apilar(persona1);
            pilaPersona.Apilar(persona2);
            Assert.Equal(persona2, pilaPersona.VerTope());
        }

        // Clase auxiliar para testing
        private class Persona
        {
            public string Nombre { get; set; } = string.Empty;
        }

        [Fact]
        public void Test_Pila_ComportamientoLIFO()
        {
            // Arrange
            var pila = new Pila<int>();
            var elementos = new[] { 1, 2, 3, 4, 5 };

            // Act - Apilar elementos
            foreach (var elemento in elementos)
            {
                pila.Apilar(elemento);
            }

            // Assert - Verificar que salen en orden inverso (LIFO)
            for (int i = elementos.Length - 1; i >= 0; i--)
            {
                Assert.Equal(elementos[i], pila.Desapilar());
            }

            Assert.True(pila.EstaVacia());
        }
    }
}