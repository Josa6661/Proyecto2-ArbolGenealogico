using System;
using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class TestArbolGenealogico
    {
        [Fact]
        public void Test_NodoFamiliar_AgregarHijo_Y_VerificarPadre()
        {
            var padre = new NodoFamiliar("Juan", "123456789", "01/01/1970", 54, "ruta/foto1.jpg", 10.0, 20.0);
            var hijo = new NodoFamiliar("Pedro", "987654321", "01/01/2000", 24, "ruta/foto2.jpg", 15.0, 25.0);

            padre.AgregarHijo(hijo);

            Assert.Equal(1, padre.Hijos.Largo());
            Assert.Equal(padre, hijo.Padre);
            Assert.Equal("Pedro", padre.Hijos.Obtener(0).Nombre);
        }


        [Fact]
        public void Test_CrearRaiz_Y_VerificarTieneRaiz()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);

            arbol.CrearRaiz(raiz);

            Assert.True(arbol.TieneRaiz());
            Assert.Equal("Abuelo", arbol.Raiz.Nombre);
        }

        [Fact]
        public void Test_ArbolSinRaiz_NoTieneRaiz()
        {
            var arbol = new ArbolGenealogico();

            Assert.False(arbol.TieneRaiz());
            Assert.Null(arbol.Raiz);
        }

        [Fact]
        public void Test_AgregarMiembro_SinRaiz_RetornaFalse()
        {
            var arbol = new ArbolGenealogico();
            var hijo = new NodoFamiliar("Pedro", "222222222", "01/01/2000", 24, "ruta/foto.jpg", 0, 0);

            bool resultado = arbol.AgregarMiembro("Juan", hijo);

            Assert.False(resultado);
        }

        [Fact]
        public void Test_AgregarMiembro_PadreNoExiste_RetornaFalse()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var hijo = new NodoFamiliar("Pedro", "222222222", "01/01/2000", 24, "ruta/foto.jpg", 0, 0);
            bool resultado = arbol.AgregarMiembro("PadreInexistente", hijo);

            Assert.False(resultado);
        }

        [Fact]
        public void Test_AgregarMiembro_ConExito()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var hijo = new NodoFamiliar("Padre", "222222222", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            bool resultado = arbol.AgregarMiembro("Abuelo", hijo);

            Assert.True(resultado);
            Assert.Equal(1, arbol.Raiz.Hijos.Largo());
        }

        [Fact]
        public void Test_BuscarPorNombre_EncontrarRaiz()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var encontrado = arbol.BuscarPorNombre("Abuelo");

            Assert.NotNull(encontrado);
            Assert.Equal("Abuelo", encontrado.Nombre);
        }

        [Fact]
        public void Test_BuscarPorNombre_EncontrarEnNivelesProfundos()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var padre = new NodoFamiliar("Padre", "222222222", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.AgregarMiembro("Abuelo", padre);

            var nieto = new NodoFamiliar("Nieto", "333333333", "01/01/2000", 24, "ruta/foto.jpg", 0, 0);
            arbol.AgregarMiembro("Padre", nieto);

            var encontrado = arbol.BuscarPorNombre("Nieto");

            Assert.NotNull(encontrado);
            Assert.Equal("Nieto", encontrado.Nombre);
        }

        [Fact]
        public void Test_BuscarPorNombre_NoExiste_RetornaNull()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var encontrado = arbol.BuscarPorNombre("Inexistente");

            Assert.Null(encontrado);
        }

        [Fact]
        public void Test_BuscarPorNombre_ArbolVacio_RetornaNull()
        {
            var arbol = new ArbolGenealogico();

            var encontrado = arbol.BuscarPorNombre("Cualquiera");

            Assert.Null(encontrado);
        }

        [Fact]
        public void Test_EliminarMiembro_EliminarRaiz()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            bool resultado = arbol.EliminarMiembro("Abuelo");

            Assert.True(resultado);
            Assert.Null(arbol.Raiz);
        }

        [Fact]
        public void Test_EliminarMiembro_EliminarHijo()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            var hijo = new NodoFamiliar("Padre", "222222222", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.AgregarMiembro("Abuelo", hijo);

            bool resultado = arbol.EliminarMiembro("Padre");

            Assert.True(resultado);
            Assert.Equal(0, arbol.Raiz.Hijos.Largo());
        }

        [Fact]
        public void Test_EliminarMiembro_NoExiste_RetornaFalse()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            bool resultado = arbol.EliminarMiembro("Inexistente");

            Assert.False(resultado);
        }

        [Fact]
        public void Test_Limpiar_EliminaTodoElArbol()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);
            arbol.AgregarMiembro("Abuelo", new NodoFamiliar("Hijo", "222222222", "01/01/1970", 54, "ruta/foto.jpg", 0, 0));

            arbol.Limpiar();

            Assert.Null(arbol.Raiz);
            Assert.False(arbol.TieneRaiz());
        }

        [Fact]
        public void Test_ObtenerTodos_ArbolVacio_RetornaListaVacia()
        {
            var arbol = new ArbolGenealogico();

            var lista = arbol.ObtenerTodos();

            Assert.NotNull(lista);
            Assert.Empty(lista);
        }

        [Fact]
        public void Test_ObtenerTodos_RetornaTodosLosMiembros()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Abuelo", "111111111", "01/01/1950", 74, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            arbol.AgregarMiembro("Abuelo", new NodoFamiliar("Padre", "222222222", "01/01/1970", 54, "ruta/foto.jpg", 0, 0));
            arbol.AgregarMiembro("Padre", new NodoFamiliar("Nieto", "333333333", "01/01/2000", 24, "ruta/foto.jpg", 0, 0));

            var lista = arbol.ObtenerTodos();

            Assert.Equal(3, lista.Count);
            Assert.Contains(lista, m => m.Nombre == "Abuelo");
            Assert.Contains(lista, m => m.Nombre == "Padre");
            Assert.Contains(lista, m => m.Nombre == "Nieto");
        }

        [Fact]
        public void Test_AgregarMultiplesHijos_AlMismoPadre()
        {
            var arbol = new ArbolGenealogico();
            var raiz = new NodoFamiliar("Padre", "111111111", "01/01/1970", 54, "ruta/foto.jpg", 0, 0);
            arbol.CrearRaiz(raiz);

            arbol.AgregarMiembro("Padre", new NodoFamiliar("Hijo1", "222222222", "01/01/2000", 24, "ruta/foto.jpg", 0, 0));
            arbol.AgregarMiembro("Padre", new NodoFamiliar("Hijo2", "333333333", "01/01/2002", 22, "ruta/foto.jpg", 0, 0));
            arbol.AgregarMiembro("Padre", new NodoFamiliar("Hijo3", "444444444", "01/01/2005", 19, "ruta/foto.jpg", 0, 0));

            Assert.Equal(3, arbol.Raiz.Hijos.Largo());
            Assert.Equal(4, arbol.ObtenerTodos().Count);
        }
    }
}