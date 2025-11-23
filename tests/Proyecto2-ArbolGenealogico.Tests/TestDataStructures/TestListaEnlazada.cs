
using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestListaEnlazada
    {
        [Fact]
        public void Test_ListaEnlazada_AgregarYEliminarElementos()
        {
            // Arrange
            var lista = new ListaEnlazada<int>();
            // Act
            lista.AgregarInicio(10);
            lista.AgregarFinal(20);
            lista.AgregarFinal(30);
            lista.EliminarPorValor(20);
            // Assert
            Assert.Equal(2, lista.Largo());
            Assert.Equal(10, lista.Obtener(0));
            Assert.Equal(30, lista.Obtener(1));
        }

        [Fact]
        public void Test_ListaVacia_LargoEsCero()
        {
            // Arrange & Act
            var lista = new ListaEnlazada<int>();
            // Assert
            Assert.Equal(0, lista.Largo());
        }

        [Fact]
        public void Test_EliminarPorIndice_EliminaCorrectamente()
        {
            // Arrange
            var lista = new ListaEnlazada<string>();
            lista.AgregarFinal("A");
            lista.AgregarFinal("B");
            lista.AgregarFinal("C");
            // Act
            lista.EliminarPorIndice(1);
            // Assert
            Assert.Equal(2, lista.Largo());
            Assert.Equal("A", lista.Obtener(0));
            Assert.Equal("C", lista.Obtener(1));
        }

        [Fact]
        public void Test_Buscar_EncuentraElemento()
        {
            // Arrange
            var lista = new ListaEnlazada<int>();
            lista.AgregarFinal(100);
            lista.AgregarFinal(200);
            lista.AgregarFinal(300);
            // Act & Assert
            Assert.Equal(1, lista.Buscar(200));
            Assert.Equal(-1, lista.Buscar(999));
        }

        [Fact]
        public void Test_Limpiar_VaciaLaLista()
        {
            // Arrange
            var lista = new ListaEnlazada<int>();
            lista.AgregarFinal(1);
            lista.AgregarFinal(2);
            // Act
            lista.Limpiar();
            // Assert
            Assert.Equal(0, lista.Largo());
        }

        [Fact]
        public void Test_ObtenerIndiceInvalido_LanzaExcepcion()
        {
            // Arrange
            var lista = new ListaEnlazada<int>();
            lista.AgregarFinal(10);
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => lista.Obtener(5));
            Assert.Throws<ArgumentOutOfRangeException>(() => lista.Obtener(-1));
        }

        [Fact]
        public void Test_EliminarPorIndiceInvalido_LanzaExcepcion()
        {
            // Arrange
            var lista = new ListaEnlazada<int>();
            lista.AgregarFinal(10);
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => lista.EliminarPorIndice(5));
        }
    }
}