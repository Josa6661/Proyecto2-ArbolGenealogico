using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestArray
    {
        [Fact]
        public void Test_Array_CrearArray_Vacio()
        {
            // Arrange
            var array = new Array<int>();
            // Act
            bool estaVacio = array.EstaVacio();
            // Assert
            Assert.True(estaVacio);
            Assert.Equal(0, array.Largo());
        }

        [Fact]
        public void Test_Array_AgregarYObtenerElementos()
        {
            // Arrange
            var array = new Array<string>();
            // Act
            array.Agregar("Elemento1");
            array.Agregar("Elemento2");
            array.Agregar("Elemento3");
            // Assert
            Assert.False(array.EstaVacio());
            Assert.Equal(3, array.Largo());
            Assert.Equal("Elemento1", array.Obtener(0));
            Assert.Equal("Elemento2", array.Obtener(1));
            Assert.Equal("Elemento3", array.Obtener(2));
        }

        [Fact]
        public void Test_Array_EliminarPorIndice_ElementoDelMedio()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);
            array.Agregar(20);
            array.Agregar(30);
            array.Agregar(40);

            // Act
            array.EliminarPorIndice(1); // Elimina el 20

            // Assert
            Assert.Equal(3, array.Largo());
            Assert.Equal(10, array.Obtener(0));
            Assert.Equal(30, array.Obtener(1));
            Assert.Equal(40, array.Obtener(2));
        }

        [Fact]
        public void Test_Array_EliminarPorIndice_PrimerElemento()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);
            array.Agregar(20);
            array.Agregar(30);

            // Act
            array.EliminarPorIndice(0);

            // Assert
            Assert.Equal(2, array.Largo());
            Assert.Equal(20, array.Obtener(0));
            Assert.Equal(30, array.Obtener(1));
        }

        [Fact]
        public void Test_Array_EliminarPorIndice_UltimoElemento()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);
            array.Agregar(20);
            array.Agregar(30);

            // Act
            array.EliminarPorIndice(2);

            // Assert
            Assert.Equal(2, array.Largo());
            Assert.Equal(10, array.Obtener(0));
            Assert.Equal(20, array.Obtener(1));
        }

        [Fact]
        public void Test_Array_Limpiar_VaciaTodosLosElementos()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);
            array.Agregar(20);
            array.Agregar(30);

            // Act
            array.Limpiar();

            // Assert
            Assert.True(array.EstaVacio());
            Assert.Equal(0, array.Largo());
        }

        [Fact]
        public void Test_Array_Obtener_IndiceNegativo_LanzaExcepcion()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => array.Obtener(-1));
            Assert.Equal("indice", exception.ParamName);
        }

        [Fact]
        public void Test_Array_Obtener_IndiceFueraDeRango_LanzaExcepcion()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);
            array.Agregar(20);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => array.Obtener(2));
            Assert.Equal("indice", exception.ParamName);
        }

        [Fact]
        public void Test_Array_EliminarPorIndice_IndiceNegativo_LanzaExcepcion()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => array.EliminarPorIndice(-1));
            Assert.Equal("indice", exception.ParamName);
        }

        [Fact]
        public void Test_Array_EliminarPorIndice_IndiceFueraDeRango_LanzaExcepcion()
        {
            // Arrange
            var array = new Array<int>();
            array.Agregar(10);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => array.EliminarPorIndice(1));
            Assert.Equal("indice", exception.ParamName);
        }

        [Fact]
        public void Test_Array_AmpliarAutomaticamente()
        {
            // Arrange
            var array = new Array<int>(2); // Capacidad inicial de 2

            // Act - Agregar más elementos que la capacidad inicial
            array.Agregar(1);
            array.Agregar(2);
            array.Agregar(3); // Aquí debería ampliar
            array.Agregar(4);
            array.Agregar(5); // Aquí debería ampliar nuevamente

            // Assert
            Assert.Equal(5, array.Largo());
            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(i + 1, array.Obtener(i));
            }
        }

        [Fact]
        public void Test_Array_CapacidadPersonalizada()
        {
            // Arrange & Act
            var array = new Array<string>(5);

            // Assert - Puede agregar 5 elementos sin ampliar
            for (int i = 0; i < 5; i++)
            {
                array.Agregar($"Item{i}");
            }

            Assert.Equal(5, array.Largo());

            // Agregar uno más para verificar que amplía correctamente
            array.Agregar("Item5");
            Assert.Equal(6, array.Largo());
            Assert.Equal("Item5", array.Obtener(5));
        }

        [Fact]
        public void Test_Array_ManejaTiposComplejos()
        {
            // Arrange
            var array = new Array<Persona>();
            var persona1 = new Persona { Id = 1, Nombre = "Juan" };
            var persona2 = new Persona { Id = 2, Nombre = "María" };
            var persona3 = new Persona { Id = 3, Nombre = "Pedro" };

            // Act
            array.Agregar(persona1);
            array.Agregar(persona2);
            array.Agregar(persona3);

            // Assert
            Assert.Equal(3, array.Largo());
            Assert.Equal(persona1, array.Obtener(0));
            Assert.Equal("María", array.Obtener(1).Nombre);

            // Eliminar y verificar
            array.EliminarPorIndice(1);
            Assert.Equal(2, array.Largo());
            Assert.Equal(persona3, array.Obtener(1));
        }

        [Fact]
        public void Test_Array_OperacionesSecuenciales()
        {
            // Arrange
            var array = new Array<int>();

            // Act & Assert - Secuencia de operaciones mixtas
            array.Agregar(10);
            array.Agregar(20);
            Assert.Equal(2, array.Largo());

            array.Agregar(30);
            array.EliminarPorIndice(1); // Elimina 20
            Assert.Equal(2, array.Largo());
            Assert.Equal(30, array.Obtener(1));

            array.Agregar(40);
            array.Agregar(50);
            Assert.Equal(4, array.Largo());

            array.Limpiar();
            Assert.True(array.EstaVacio());

            array.Agregar(100);
            Assert.Equal(1, array.Largo());
            Assert.Equal(100, array.Obtener(0));
        }

        [Fact]
        public void Test_Array_GranCantidadDeElementos()
        {
            // Arrange
            var array = new Array<int>();
            const int cantidad = 10000;

            // Act
            for (int i = 0; i < cantidad; i++)
            {
                array.Agregar(i);
            }

            // Assert
            Assert.Equal(cantidad, array.Largo());

            // Verificar algunos elementos aleatorios
            Assert.Equal(0, array.Obtener(0));
            Assert.Equal(4999, array.Obtener(4999));
            Assert.Equal(9999, array.Obtener(9999));

            // Eliminar elementos del medio y verificar
            array.EliminarPorIndice(5000);
            Assert.Equal(cantidad - 1, array.Largo());
            Assert.Equal(5001, array.Obtener(5000)); // El 5001 ahora está en la posición 5000
        }

        // Clase auxiliar para testing
        private class Persona
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
        }
    }
}