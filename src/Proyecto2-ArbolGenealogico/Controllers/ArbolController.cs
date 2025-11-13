using Proyecto2_ArbolGenealogico.DataStructures;
using Proyecto2_ArbolGenealogico.Services;
using System.Collections.Generic;
using System.Diagnostics;

namespace Proyecto2_ArbolGenealogico.Controllers
{
    public class ArbolController
    {
        private readonly ArbolService _service;

        public ArbolController()
        {
            // Usamos el Singleton
            _service = ArbolService.Instancia;
        }

        // Crear la raíz del árbol
        public void CrearRaiz(string nombre, string cedula, string fechaNacimiento, int edad, string fotoRuta, double latitud, double longitud)
        {
            var nuevo = new NodoFamiliar(nombre, cedula, fechaNacimiento, edad, fotoRuta, latitud, longitud);
            bool creada = _service.CrearRaiz(nuevo);

            Debug.WriteLine(creada
                ? $"[Controller] Raíz '{nombre}' creada correctamente."
                : $"[Controller] Ya existe una raíz, no se pudo crear '{nombre}'.");
        }

        // Agregar un nuevo miembro a un padre
        public void AgregarMiembro(string nombrePadre, string nombre, string cedula, string fechaNacimiento, int edad, string fotoRuta, double latitud, double longitud)
        {
            var hijo = new NodoFamiliar(nombre, cedula, fechaNacimiento, edad, fotoRuta, latitud, longitud);
            bool agregado = _service.AgregarMiembro(nombrePadre, hijo);

            Debug.WriteLine(agregado
                ? $"[Controller] Hijo '{nombre}' agregado a '{nombrePadre}'."
                : $"[Controller] No se pudo agregar '{nombre}', padre '{nombrePadre}' no encontrado.");
        }

        // Buscar miembro
        public NodoFamiliar BuscarMiembro(string nombre)
        {
            var encontrado = _service.BuscarMiembro(nombre);
            Debug.WriteLine(encontrado != null
                ? $"[Controller] Miembro '{nombre}' encontrado."
                : $"[Controller] Miembro '{nombre}' no encontrado.");
            return encontrado;
        }

        // Eliminar miembro
        public void EliminarMiembro(string nombre)
        {
            bool eliminado = _service.EliminarMiembro(nombre);
            Debug.WriteLine(eliminado
                ? $"[Controller] Miembro '{nombre}' eliminado correctamente."
                : $"[Controller] No se pudo eliminar '{nombre}', no encontrado.");
        }

        // Limpiar todo el árbol
        public void Limpiar()
        {
            _service.Limpiar();
            Debug.WriteLine("[Controller] Árbol limpiado completamente.");
        }

        // Obtener todos los miembros
        public ListaEnlazada<NodoFamiliar> ObtenerTodos()
        {
            var lista = _service.ObtenerTodos();
            Debug.WriteLine($"[Controller] Se obtuvieron {lista.Largo()} miembros del árbol.");
            return lista;
        }
    }
}
