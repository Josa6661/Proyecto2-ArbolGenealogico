using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestArbolGenealogico
    {
        [Fact]
        public void Test_AgregarConyuge_ConExito()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var conyuge = new NodoFamiliar("Maria", "222222222", "01/01/1972", 52, "ruta/foto.jpg", 0, 0);
            bool resultado = arbol.AgregarConyuge("Juan", conyuge);

            Assert.True(resultado);
            Assert.NotNull(raiz.Conyuge);
            Assert.Equal("Maria", raiz.Conyuge.Nombre);
        }

        [Fact]
        public void Test_AgregarConyuge_MiembroNoExiste_RetornaFalse()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var conyuge = new NodoFamiliar("Maria", "222222222", "01/01/1972", 52, "ruta/foto.jpg", 0, 0);
            bool resultado = arbol.AgregarConyuge("Pedro", conyuge);

            Assert.False(resultado);
        }

        [Fact]
        public void Test_AgregarConyuge_YaTieneConyuge_RetornaFalse()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var conyuge1 = new NodoFamiliar("Maria", "222222222", "01/01/1972", 52, "ruta/foto.jpg", 0, 0);
            arbol.AgregarConyuge("Juan", conyuge1);

            var conyuge2 = new NodoFamiliar("Ana", "333333333", "01/01/1973", 51, "ruta/foto.jpg", 0, 0);
            bool resultado = arbol.AgregarConyuge("Juan", conyuge2);

            Assert.False(resultado);
            Assert.Equal("Maria", raiz.Conyuge.Nombre);
        }

        [Fact]
        public void Test_BuscarPorNombre_EncontrarConyuge()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var conyuge = new NodoFamiliar("Maria", "222222222", "01/01/1972", 52, "ruta/foto.jpg", 0, 0);
            arbol.AgregarConyuge("Juan", conyuge);

            var encontrado = arbol.BuscarPorNombre("Maria");

            Assert.NotNull(encontrado);
            Assert.Equal("Maria", encontrado.Nombre);
        }

        [Fact]
        public void Test_BuscarPorCedula_Encontrar()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var encontrado = arbol.BuscarPorCedula("111111111");

            Assert.NotNull(encontrado);
            Assert.Equal("Juan", encontrado.Nombre);
        }

        [Fact]
        public void Test_BuscarPorCedula_EncontrarConyuge()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var conyuge = new NodoFamiliar("Maria", "222222222", "01/01/1972", 52, "ruta/foto.jpg", 0, 0);
            arbol.AgregarConyuge("Juan", conyuge);

            var encontrado = arbol.BuscarPorCedula("222222222");

            Assert.NotNull(encontrado);
            Assert.Equal("Maria", encontrado.Nombre);
        }

        [Fact]
        public void Test_AgregarPadreARaiz_ConExito()
        {
            var arbol = new ArbolGenealogico();
            var raizActual = new NodoFamiliar("Padre", "222222222", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raizActual);

            var nuevoPadre = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.AgregarPadreARaiz(nuevoPadre);

            Assert.Equal("Abuelo", arbol.Raiz.Nombre);
            Assert.Equal(1, arbol.Raiz.Hijos.Largo());
            Assert.Equal("Padre", arbol.Raiz.Hijos.Obtener(0).Nombre);
        }

        [Fact]
        public void Test_ObtenerNodosJerarquicos_NoIncluyeConyuge()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var conyuge = new NodoFamiliar("Maria", "222222222", "01/01/1972", 52, "ruta/foto.jpg", 0, 0);
            arbol.AgregarConyuge("Juan", conyuge);

            var lista = arbol.ObtenerNodosJerarquicos();

            Assert.Equal(1, lista.Largo());
            Assert.Equal("Juan", lista.Obtener(0).Nombre);
        }

        [Fact]
        public void Test_ObtenerTodos_IncluyeConyuge()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Juan", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var conyuge = new NodoFamiliar("Maria", "222222222", "01/01/1972", 52, "ruta/foto.jpg", 0, 0);
            arbol.AgregarConyuge("Juan", conyuge);

            var lista = arbol.ObtenerTodos();

            Assert.Equal(2, lista.Largo());
            bool tieneJuan = false, tieneMaria = false;
            for (int i = 0; i < lista.Largo(); i++)
            {
                if (lista.Obtener(i).Nombre == "Juan") tieneJuan = true;
                if (lista.Obtener(i).Nombre == "Maria") tieneMaria = true;
            }
            Assert.True(tieneJuan);
            Assert.True(tieneMaria);
        }

        [Fact]
        public void Test_ObtenerHijosDeNodo_RetornaListaCorrecta()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Padre", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            arbol.AgregarMiembro("Padre", new NodoFamiliar("Hijo1", "222222222", "01/01/2000", 24, "ruta/foto.jpg", 0, 0));
            arbol.AgregarMiembro("Padre", new NodoFamiliar("Hijo2", "333333333", "01/01/2002", 22, "ruta/foto.jpg", 0, 0));

            var hijos = arbol.ObtenerHijosDeNodo("Padre");

            Assert.Equal(2, hijos.Largo());
        }
    }
}