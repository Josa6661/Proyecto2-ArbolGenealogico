using Xunit;
using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.Tests.DataStructures
{
    public class NodoFamiliarTests
    {
        // Helper method para crear nodos de prueba
        private NodoFamiliar CrearNodoEjemplo(string nombre = "Juan", string cedula = "123456789")
        {
            return new NodoFamiliar(nombre, cedula, "01/01/1990", 30, "foto.jpg", 9.9281, -84.0907);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_InicializaCorrectamente()
        {
            var nodo = CrearNodoEjemplo();

            Assert.Equal("Juan", nodo.Nombre);
            Assert.Equal("123456789", nodo.Cedula);
            Assert.Equal("01/01/1990", nodo.FechaNacimiento);
            Assert.Equal(30, nodo.Edad);
            Assert.NotNull(nodo.Padres);
            Assert.NotNull(nodo.Hijos);
            Assert.Null(nodo.Conyuge);
            Assert.Equal(0, nodo.Padres.Largo());
            Assert.Equal(0, nodo.Hijos.Largo());
        }

        #endregion

        #region AgregarHijo Tests

        [Fact]
        public void AgregarHijo_AgregaHijoYEstablecePadre()
        {
            var padre = CrearNodoEjemplo("Padre", "111");
            var hijo = CrearNodoEjemplo("Hijo", "222");

            padre.AgregarHijo(hijo);

            Assert.Equal(1, padre.Hijos.Largo());
            Assert.Equal(hijo, padre.Hijos.Obtener(0));
            Assert.Equal(1, hijo.Padres.Largo());
            Assert.Equal(padre, hijo.Padres.Obtener(0));
        }

        [Fact]
        public void AgregarHijo_MultiplesHijos_TodosSeAgregan()
        {
            var padre = CrearNodoEjemplo("Padre", "111");
            var hijo1 = CrearNodoEjemplo("Hijo1", "222");
            var hijo2 = CrearNodoEjemplo("Hijo2", "333");

            padre.AgregarHijo(hijo1);
            padre.AgregarHijo(hijo2);

            Assert.Equal(2, padre.Hijos.Largo());
            Assert.Equal(1, hijo1.Padres.Largo());
            Assert.Equal(1, hijo2.Padres.Largo());
        }

        [Fact]
        public void AgregarHijo_DosPadres_AmbosSeEstablecen()
        {
            var padre = CrearNodoEjemplo("Padre", "111");
            var madre = CrearNodoEjemplo("Madre", "222");
            var hijo = CrearNodoEjemplo("Hijo", "333");

            padre.AgregarHijo(hijo);
            madre.AgregarHijo(hijo);

            Assert.Equal(2, hijo.Padres.Largo());
            Assert.Equal(1, padre.Hijos.Largo());
            Assert.Equal(1, madre.Hijos.Largo());
        }

        [Fact]
        public void AgregarHijo_NoDuplicaPadre()
        {
            var padre = CrearNodoEjemplo("Padre", "111");
            var hijo = CrearNodoEjemplo("Hijo", "222");

            padre.AgregarHijo(hijo);
            padre.AgregarHijo(hijo); // Intentar agregar de nuevo

            Assert.Equal(2, padre.Hijos.Largo()); // Se agrega como hijo dos veces
            Assert.Equal(1, hijo.Padres.Largo()); // Pero solo un padre
        }

        [Fact]
        public void AgregarHijo_NoAgregaTercerPadre()
        {
            var padre1 = CrearNodoEjemplo("Padre1", "111");
            var padre2 = CrearNodoEjemplo("Padre2", "222");
            var padre3 = CrearNodoEjemplo("Padre3", "333");
            var hijo = CrearNodoEjemplo("Hijo", "444");

            padre1.AgregarHijo(hijo);
            padre2.AgregarHijo(hijo);
            padre3.AgregarHijo(hijo);

            Assert.Equal(2, hijo.Padres.Largo()); // Solo 2 padres
            Assert.Equal(1, padre3.Hijos.Largo()); // Pero se agrega como hijo
        }

        #endregion

        #region EstablecerConyuge Tests

        [Fact]
        public void EstablecerConyuge_EstableceRelacionBidireccional()
        {
            var esposo = CrearNodoEjemplo("Esposo", "111");
            var esposa = CrearNodoEjemplo("Esposa", "222");

            esposo.EstablecerConyuge(esposa);

            Assert.Equal(esposa, esposo.Conyuge);
            Assert.Equal(esposo, esposa.Conyuge);
        }

        [Fact]
        public void EstablecerConyuge_PuedeReemplazar()
        {
            var persona = CrearNodoEjemplo("Persona", "111");
            var conyuge1 = CrearNodoEjemplo("Conyuge1", "222");
            var conyuge2 = CrearNodoEjemplo("Conyuge2", "333");

            persona.EstablecerConyuge(conyuge1);
            persona.EstablecerConyuge(conyuge2);

            Assert.Equal(conyuge2, persona.Conyuge);
        }

        #endregion

        #region AgregarPadre Tests

        [Fact]
        public void AgregarPadre_AgregaPrimerPadre()
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");
            var padre = CrearNodoEjemplo("Padre", "222");

            bool resultado = hijo.AgregarPadre(padre);

            Assert.True(resultado);
            Assert.Equal(1, hijo.Padres.Largo());
            Assert.Equal(padre, hijo.Padres.Obtener(0));
        }

        [Fact]
        public void AgregarPadre_AgregaSegundoPadre()
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");
            var padre = CrearNodoEjemplo("Padre", "222");
            var madre = CrearNodoEjemplo("Madre", "333");

            hijo.AgregarPadre(padre);
            bool resultado = hijo.AgregarPadre(madre);

            Assert.True(resultado);
            Assert.Equal(2, hijo.Padres.Largo());
        }

        [Fact]
        public void AgregarPadre_RechazaTercerPadre()
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");
            var padre1 = CrearNodoEjemplo("Padre1", "222");
            var padre2 = CrearNodoEjemplo("Padre2", "333");
            var padre3 = CrearNodoEjemplo("Padre3", "444");

            hijo.AgregarPadre(padre1);
            hijo.AgregarPadre(padre2);
            bool resultado = hijo.AgregarPadre(padre3);

            Assert.False(resultado);
            Assert.Equal(2, hijo.Padres.Largo());
        }

        [Fact]
        public void AgregarPadre_RechazaDuplicado()
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");
            var padre = CrearNodoEjemplo("Padre", "222");

            hijo.AgregarPadre(padre);
            bool resultado = hijo.AgregarPadre(padre);

            Assert.False(resultado);
            Assert.Equal(1, hijo.Padres.Largo());
        }

        #endregion

        #region Métodos de Consulta Tests

        [Fact]
        public void ObtenerPrimerPadre_RetornaPrimerPadre()
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");
            var padre = CrearNodoEjemplo("Padre", "222");
            var madre = CrearNodoEjemplo("Madre", "333");

            hijo.AgregarPadre(padre);
            hijo.AgregarPadre(madre);

            Assert.Equal(padre, hijo.ObtenerPrimerPadre());
        }

        [Fact]
        public void ObtenerPrimerPadre_RetornaNullSinPadres()
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");

            Assert.Null(hijo.ObtenerPrimerPadre());
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        public void TienePadres_RetornaCorrectamente(int numeroPadres, bool esperado)
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");

            for (int i = 0; i < numeroPadres; i++)
            {
                hijo.AgregarPadre(CrearNodoEjemplo($"Padre{i}", $"{200 + i}"));
            }

            Assert.Equal(esperado, hijo.TienePadres());
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        public void TieneDosPadres_RetornaCorrectamente(int numeroPadres, bool esperado)
        {
            var hijo = CrearNodoEjemplo("Hijo", "111");

            for (int i = 0; i < numeroPadres; i++)
            {
                hijo.AgregarPadre(CrearNodoEjemplo($"Padre{i}", $"{200 + i}"));
            }

            Assert.Equal(esperado, hijo.TieneDosPadres());
        }

        #endregion

        #region Tests de Integración Simplificados

        [Fact]
        public void EscenarioCompleto_FamiliaConDosPadresYDosHijos()
        {
            var padre = CrearNodoEjemplo("Padre", "111");
            var madre = CrearNodoEjemplo("Madre", "222");
            var hijo1 = CrearNodoEjemplo("Hijo1", "333");
            var hijo2 = CrearNodoEjemplo("Hijo2", "444");

            padre.EstablecerConyuge(madre);
            padre.AgregarHijo(hijo1);
            madre.AgregarHijo(hijo1);
            padre.AgregarHijo(hijo2);
            madre.AgregarHijo(hijo2);

            // Verificaciones
            Assert.Equal(madre, padre.Conyuge);
            Assert.Equal(2, padre.Hijos.Largo());
            Assert.Equal(2, madre.Hijos.Largo());
            Assert.True(hijo1.TieneDosPadres());
            Assert.True(hijo2.TieneDosPadres());
        }

        #endregion
    }
}