//using System;
//using Xunit;
//using Proyecto2_ArbolGenealogico.DataStructures;

//namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
//{
//    public class TestNodoFamiliar 
//    {
//        [Fact]
//        public void Test_NodoFamiliar_AgregarHijo_Y_VerificarPadre()
//        {
//            var padre = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar("Juan", "123456789", "01/01/1970", 54, "ruta/foto1.jpg", 10.0, 20.0);
//            var hijo = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar("Pedro", "987654321", "01/01/2000", 24, "ruta/foto2.jpg", 15.0, 25.0);

//            padre.AgregarHijo(hijo);

//            Assert.Equal(1, padre.Hijos.Largo());
//            Assert.Equal(padre, hijo.Padre);
//            Assert.Equal("Pedro", padre.Hijos.Obtener(0).Nombre);
//        }

//        [Fact]
//        public void Test_Constructor_InicializaCorrectamente()
//        {
//            var nodo = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Juan", "123456789", "01/01/1970", 54, "ruta/foto.jpg", 10.0, 20.0);

//            Assert.Equal("Juan", nodo.Nombre);
//            Assert.Equal("123456789", nodo.Cedula);
//            Assert.Equal("01/01/1970", nodo.FechaNacimiento);
//            Assert.Equal(54, nodo.Edad);
//            Assert.Equal("ruta/foto.jpg", nodo.FotoRuta);
//            Assert.Equal(10.0, nodo.Latitud);
//            Assert.Equal(20.0, nodo.Longitud);
//            Assert.Null(nodo.Padre);
//            Assert.NotNull(nodo.Hijos);
//            Assert.Equal(0, nodo.Hijos.Largo());
//        }

//        [Fact]
//        public void Test_AgregarMultiplesHijos_TodosSeAgregan()
//        {
//            var padre = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Padre", "111111111", "01/01/1970", 54, "foto.jpg", 0, 0);
//            var hijo1 = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Hijo1", "222222222", "01/01/2000", 24, "foto.jpg", 0, 0);
//            var hijo2 = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Hijo2", "333333333", "01/01/2002", 22, "foto.jpg", 0, 0);
//            var hijo3 = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Hijo3", "444444444", "01/01/2005", 19, "foto.jpg", 0, 0);

//            padre.AgregarHijo(hijo1);
//            padre.AgregarHijo(hijo2);
//            padre.AgregarHijo(hijo3);

//            Assert.Equal(3, padre.Hijos.Largo());
//            Assert.Equal("Hijo1", padre.Hijos.Obtener(0).Nombre);
//            Assert.Equal("Hijo2", padre.Hijos.Obtener(1).Nombre);
//            Assert.Equal("Hijo3", padre.Hijos.Obtener(2).Nombre);
//        }

//        [Fact]
//        public void Test_AgregarHijo_CadaHijoTienePadreCorrect()
//        {
//            var padre = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Padre", "111111111", "01/01/1970", 54, "foto.jpg", 0, 0);
//            var hijo1 = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Hijo1", "222222222", "01/01/2000", 24, "foto.jpg", 0, 0);
//            var hijo2 = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Hijo2", "333333333", "01/01/2002", 22, "foto.jpg", 0, 0);

//            padre.AgregarHijo(hijo1);
//            padre.AgregarHijo(hijo2);

//            Assert.Equal(padre, hijo1.Padre);
//            Assert.Equal(padre, hijo2.Padre);
//        }

//        [Fact]
//        public void Test_NodoRecienCreado_NoPadre_ListaHijosVacia()
//        {
//            var nodo = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Juan", "123456789", "01/01/1970", 54, "foto.jpg", 0, 0);

//            Assert.Null(nodo.Padre);
//            Assert.NotNull(nodo.Hijos);
//            Assert.Equal(0, nodo.Hijos.Largo());
//        }


//        [Fact]
//        public void Test_PropiedadesGeograficas_SeAlmacenanCorrectamente()
//        {
//            var nodo = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Juan", "123", "01/01/1970", 54, "foto.jpg", 9.9281, -84.0907);

//            Assert.Equal(9.9281, nodo.Latitud);
//            Assert.Equal(-84.0907, nodo.Longitud);
//        }

//        [Fact]
//        public void Test_ModificarPropiedades_SeActualizanCorrectamente()
//        {
//            var nodo = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Juan", "123456789", "01/01/1970", 54, "foto.jpg", 0, 0);

//            nodo.Nombre = "Juan Carlos";
//            nodo.Edad = 55;
//            nodo.FotoRuta = "nueva/ruta/foto.jpg";

//            Assert.Equal("Juan Carlos", nodo.Nombre);
//            Assert.Equal(55, nodo.Edad);
//            Assert.Equal("nueva/ruta/foto.jpg", nodo.FotoRuta);
//        }

//        [Fact]
//        public void Test_HijoConHijos_CrearGeneraciones()
//        {
//            // Abuelo
//            var abuelo = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Abuelo", "111", "01/01/1950", 74, "foto.jpg", 0, 0);

//            // Padre
//            var padre = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Padre", "222", "01/01/1975", 49, "foto.jpg", 0, 0);

//            // Nieto
//            var nieto = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Nieto", "333", "01/01/2005", 19, "foto.jpg", 0, 0);

//            abuelo.AgregarHijo(padre);
//            padre.AgregarHijo(nieto);

//            Assert.Equal(1, abuelo.Hijos.Largo());
//            Assert.Equal(1, padre.Hijos.Largo());
//            Assert.Equal(abuelo, padre.Padre);
//            Assert.Equal(padre, nieto.Padre);
//            Assert.Null(abuelo.Padre);
//        }

//        [Fact]
//        public void Test_ValoresVaciosONulos_NoGeneranError()
//        {
//            var nodo = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "", "", "", 0, "", 0, 0);

//            Assert.NotNull(nodo);
//            Assert.Equal("", nodo.Nombre);
//            Assert.Equal(0, nodo.Edad);
//        }

//        [Fact]
//        public void Test_CedulasUnicas_EnDiferentesNodos()
//        {
//            var nodo1 = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Juan", "123456789", "01/01/1970", 54, "foto.jpg", 0, 0);
//            var nodo2 = new Proyecto2_ArbolGenealogico.DataStructures.NodoFamiliar(
//                "Pedro", "987654321", "01/01/1980", 44, "foto.jpg", 0, 0);

//            Assert.NotEqual(nodo1.Cedula, nodo2.Cedula);
//        }
//    }
//}