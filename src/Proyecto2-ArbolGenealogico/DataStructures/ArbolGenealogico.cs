using Proyecto2_ArbolGenealogico.DataStructures;

namespace Proyecto2_ArbolGenealogico.DataStructures
{
    public class ArbolGenealogico
    {
        public NodoFamiliar Raiz { get; private set; }

        public ArbolGenealogico()
        {
            Raiz = null;
        }

        public void CrearRaiz(NodoFamiliar miembro)
        {
            Raiz = miembro;
        }

        public bool TieneRaiz()
        {
            return Raiz != null;
        }

        public NodoFamiliar BuscarPorNombre(string nombre)
        {
            // Buscar en TODOS los nodos del árbol (incluyendo ancestros)
            var todosLosNodos = ObtenerTodos();
            for (int i = 0; i < todosLosNodos.Largo(); i++)
            {
                var nodo = todosLosNodos.Obtener(i);
                if (string.Equals(nodo.Nombre, nombre, System.StringComparison.OrdinalIgnoreCase))
                    return nodo;
            }
            return null;
        }

        private NodoFamiliar BuscarPorNombreRecursivo(NodoFamiliar actual, string nombre)
        {
            if (actual == null)
                return null;

            if (actual.Nombre == nombre)
                return actual;

            // Buscar en el cónyuge
            if (actual.Conyuge != null && actual.Conyuge.Nombre == nombre)
                return actual.Conyuge;

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                var encontrado = BuscarPorNombreRecursivo(actual.Hijos.Obtener(i), nombre);
                if (encontrado != null)
                    return encontrado;
            }

            return null;
        }

        public bool AgregarMiembro(string nombrePadre, NodoFamiliar hijo)
        {
            // Validación de raíz
            if (Raiz == null)
                return false;

            var padre = BuscarPorNombre(nombrePadre);
            if (padre != null)
            {
                padre.AgregarHijo(hijo);
                return true;
            }
            return false;
        }

        public bool EliminarMiembro(string nombre)
        {
            if (Raiz != null && Raiz.Nombre == nombre)
            {
                // Si la raíz tiene padres, promover al primer padre como nueva raíz
                if (Raiz.Padres.Largo() > 0)
                {
                    var nuevaRaiz = Raiz.Padres.Obtener(0);
                    
                    // Eliminar la raíz actual de la lista de hijos de TODOS sus padres
                    for (int p = 0; p < Raiz.Padres.Largo(); p++)
                    {
                        var padre = Raiz.Padres.Obtener(p);
                        for (int i = 0; i < padre.Hijos.Largo(); i++)
                        {
                            if (padre.Hijos.Obtener(i).Nombre == nombre)
                            {
                                padre.Hijos.EliminarPorIndice(i);
                                break;
                            }
                        }
                    }
                    
                    Raiz = nuevaRaiz;
                }
                else
                {
                    // Si no tiene padres, eliminar todo el árbol
                    Raiz = null;
                }
                return true;
            }

            return EliminarRecursivo(Raiz, nombre);
        }

        private bool EliminarRecursivo(NodoFamiliar actual, string nombre)
        {
            if (actual == null)
                return false;

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                var hijo = actual.Hijos.Obtener(i);

                if (hijo.Nombre == nombre)
                {
                    actual.Hijos.EliminarPorIndice(i);
                    return true;
                }

                if (EliminarRecursivo(hijo, nombre))
                    return true;
            }

            return false;
        }

        public void Limpiar()
        {
            Raiz = null;
        }

        public ListaEnlazada<NodoFamiliar> ObtenerTodos()
        {
            var lista = new ListaEnlazada<NodoFamiliar>();
            var visitados = new ListaEnlazada<NodoFamiliar>();
            
            if (Raiz == null)
                return lista;
            
            // Primero, ir hacia arriba para encontrar las raíces reales (nodos sin padres)
            var raicesReales = new ListaEnlazada<NodoFamiliar>();
            EncontrarRaicesReales(Raiz, raicesReales, visitados);
            
            // Luego recorrer desde cada raíz real
            for (int i = 0; i < raicesReales.Largo(); i++)
            {
                ObtenerTodosDesdeNodo(raicesReales.Obtener(i), lista);
            }
            
            return lista;
        }
        
        // Encuentra todas las raíces reales (nodos sin padres) subiendo desde la raíz actual
        private void EncontrarRaicesReales(NodoFamiliar nodo, ListaEnlazada<NodoFamiliar> raices, ListaEnlazada<NodoFamiliar> visitados)
        {
            if (nodo == null || visitados.Contiene(nodo))
                return;
                
            visitados.AgregarFinal(nodo);
            
            // Si no tiene padres, es una raíz real
            if (nodo.Padres.Largo() == 0)
            {
                if (!raices.Contiene(nodo))
                    raices.AgregarFinal(nodo);
            }
            else
            {
                // Subir a los padres
                for (int i = 0; i < nodo.Padres.Largo(); i++)
                {
                    EncontrarRaicesReales(nodo.Padres.Obtener(i), raices, visitados);
                }
            }
            
            // También revisar el cónyuge
            if (nodo.Conyuge != null)
            {
                EncontrarRaicesReales(nodo.Conyuge, raices, visitados);
            }
        }
        
        // Obtiene todos los nodos desde un nodo específico (descendientes y cónyuge)
        private void ObtenerTodosDesdeNodo(NodoFamiliar nodo, ListaEnlazada<NodoFamiliar> lista)
        {
            if (nodo == null || lista.Contiene(nodo))
                return;
                
            lista.AgregarFinal(nodo);
            
            // Agregar cónyuge
            if (nodo.Conyuge != null && !lista.Contiene(nodo.Conyuge))
            {
                lista.AgregarFinal(nodo.Conyuge);
            }
            
            // Recorrer hijos
            for (int i = 0; i < nodo.Hijos.Largo(); i++)
            {
                ObtenerTodosDesdeNodo(nodo.Hijos.Obtener(i), lista);
            }
        }

        // Obtiene solo los nodos en la jerarquía del árbol (sin cónyuges)
        public ListaEnlazada<NodoFamiliar> ObtenerNodosJerarquicos()
        {
            var lista = new ListaEnlazada<NodoFamiliar>();
            ObtenerNodosJerarquicosRecursivo(Raiz, lista);
            return lista;
        }

        private void ObtenerNodosJerarquicosRecursivo(NodoFamiliar actual, ListaEnlazada<NodoFamiliar> lista)
        {
            if (actual == null)
                return;

            lista.AgregarFinal(actual);

            // NO agregar el cónyuge, solo los descendientes
            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                ObtenerNodosJerarquicosRecursivo(actual.Hijos.Obtener(i), lista);
            }
        }

        private void ObtenerTodosRecursivo(NodoFamiliar actual, ListaEnlazada<NodoFamiliar> lista)
        {
            if (actual == null)
                return;

            lista.AgregarFinal(actual);

            // Agregar el cónyuge si existe y no está ya en la lista
            if (actual.Conyuge != null && !lista.Contiene(actual.Conyuge))
            {
                lista.AgregarFinal(actual.Conyuge);
            }

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                ObtenerTodosRecursivo(actual.Hijos.Obtener(i), lista);
            }
        }

        // Buscar por cédula para evitar duplicados
        public NodoFamiliar BuscarPorCedula(string cedula)
        {
            return BuscarPorCedulaRecursivo(Raiz, cedula);
        }

        private NodoFamiliar BuscarPorCedulaRecursivo(NodoFamiliar actual, string cedula)
        {
            if (actual == null)
                return null;

            if (actual.Cedula == cedula)
                return actual;

            // Buscar en el cónyuge
            if (actual.Conyuge != null && actual.Conyuge.Cedula == cedula)
                return actual.Conyuge;

            for (int i = 0; i < actual.Hijos.Largo(); i++)
            {
                var encontrado = BuscarPorCedulaRecursivo(actual.Hijos.Obtener(i), cedula);
                if (encontrado != null)
                    return encontrado;
            }

            return null;
        }

        // Agregar un padre a la raíz actual (convierte la raíz actual en hijo del nuevo padre)
        public void AgregarPadreARaiz(NodoFamiliar nuevoPadre)
        {
            if (Raiz != null)
            {
                // La raíz actual se convierte en hijo del nuevo padre
                nuevoPadre.AgregarHijo(Raiz);
            }
            // El nuevo padre se convierte en la nueva raíz
            Raiz = nuevoPadre;
        }

        // Agregar cónyuge a un miembro existente
        public bool AgregarConyuge(string nombreMiembro, NodoFamiliar conyuge)
        {
            var miembro = BuscarPorNombre(nombreMiembro);
            if (miembro == null)
                return false;

            if (miembro.Conyuge != null)
                return false; // Ya tiene cónyuge

            miembro.EstablecerConyuge(conyuge);
            return true;
        }
        // Busca todos los hijos de un nodo específico (útil para hermanos)
        public ListaEnlazada<NodoFamiliar> ObtenerHijosDeNodo(string nombrePadre)
        {
            var resultado = new ListaEnlazada<NodoFamiliar>();
            var padre = BuscarPorNombre(nombrePadre);

            if (padre != null)
            {
                for (int i = 0; i < padre.Hijos.Largo(); i++)
                {
                    resultado.AgregarFinal(padre.Hijos.Obtener(i));
                }
            }

            return resultado;
        }

        // Agregar padre/madre a un miembro existente
        public bool AgregarPadreAMiembro(string nombreMiembro, NodoFamiliar nuevoPadre)
        {
            var miembro = BuscarPorNombre(nombreMiembro);
            if (miembro == null)
                return false;

            // Validar que no tenga ya 2 padres
            if (miembro.Padres.Largo() >= 2)
                return false;

            NodoFamiliar primerPadre = null;
            bool debeEstablecerConyuges = false;

            // Si ya tiene 1 padre, necesitaremos establecer relación de cónyuge DESPUÉS
            if (miembro.Padres.Largo() == 1)
            {
                primerPadre = miembro.Padres.Obtener(0);
                
                // Solo establecer como cónyuges si ninguno tiene ya otro cónyuge
                if (primerPadre.Conyuge == null && nuevoPadre.Conyuge == null)
                {
                    debeEstablecerConyuges = true;
                }
            }

            // Agregar el nuevo padre al miembro
            if (!miembro.AgregarPadre(nuevoPadre))
                return false;

            // Agregar el miembro como hijo del nuevo padre
            nuevoPadre.AgregarHijo(miembro);

            // AHORA establecer como cónyuges DESPUÉS de haber agregado la relación padre-hijo
            if (debeEstablecerConyuges && primerPadre != null)
            {
                EstablecerConyugesYSincronizarHijos(primerPadre, nuevoPadre);
            }

            return true;
        }

        // Agregar ambos padres (madre y padre) a un miembro existente
        public bool AgregarPadresAMiembro(string nombreMiembro, NodoFamiliar padre, NodoFamiliar madre)
        {
            var miembro = BuscarPorNombre(nombreMiembro);
            if (miembro == null)
                return false;

            // Validar que no tenga ya padres
            if (miembro.Padres.Largo() > 0)
                return false;

            // Establecer a padre y madre como cónyuges
            padre.EstablecerConyuge(madre);

            // Agregar ambos padres al miembro
            miembro.AgregarPadre(padre);
            miembro.AgregarPadre(madre);

            // Agregar el miembro como hijo de ambos padres
            padre.AgregarHijo(miembro);
            madre.AgregarHijo(miembro);

            return true;
        }

        // Establece dos nodos como cónyuges y sincroniza sus hijos
        private void EstablecerConyugesYSincronizarHijos(NodoFamiliar padre1, NodoFamiliar padre2)
        {
            // Establecer relación de cónyuge (sin sincronizar hijos automáticamente)
            padre1.Conyuge = padre2;
            padre2.Conyuge = padre1;

            // Sincronizar hijos manualmente
            // Agregar los hijos de padre1 a padre2
            for (int i = 0; i < padre1.Hijos.Largo(); i++)
            {
                var hijo = padre1.Hijos.Obtener(i);
                
                // Verificar si padre2 ya tiene este hijo
                bool yaEsHijo = false;
                for (int j = 0; j < padre2.Hijos.Largo(); j++)
                {
                    if (padre2.Hijos.Obtener(j) == hijo)
                    {
                        yaEsHijo = true;
                        break;
                    }
                }
                
                if (!yaEsHijo)
                {
                    padre2.Hijos.AgregarFinal(hijo);
                    // Agregar padre2 como padre del hijo si tiene espacio
                    if (hijo.Padres.Largo() < 2)
                    {
                        hijo.Padres.AgregarFinal(padre2);
                    }
                }
            }

            // Agregar los hijos de padre2 a padre1
            for (int i = 0; i < padre2.Hijos.Largo(); i++)
            {
                var hijo = padre2.Hijos.Obtener(i);
                
                // Verificar si padre1 ya tiene este hijo
                bool yaEsHijo = false;
                for (int j = 0; j < padre1.Hijos.Largo(); j++)
                {
                    if (padre1.Hijos.Obtener(j) == hijo)
                    {
                        yaEsHijo = true;
                        break;
                    }
                }
                
                if (!yaEsHijo)
                {
                    padre1.Hijos.AgregarFinal(hijo);
                    // Agregar padre1 como padre del hijo si tiene espacio
                    if (hijo.Padres.Largo() < 2)
                    {
                        hijo.Padres.AgregarFinal(padre1);
                    }
                }
            }
        }

        // Obtener todos los nodos que son raíces (no tienen padres)
        public ListaEnlazada<NodoFamiliar> ObtenerNodosRaiz()
        {
            var raices = new ListaEnlazada<NodoFamiliar>();
            var todosNodos = ObtenerTodos();

            for (int i = 0; i < todosNodos.Largo(); i++)
            {
                var nodo = todosNodos.Obtener(i);
                if (nodo.Padres.Largo() == 0)
                {
                    raices.AgregarFinal(nodo);
                }
            }

            return raices;
        }

        // Actualiza la raíz del árbol para que apunte al ancestro más antiguo
        // Esto es necesario después de agregar nodos para asegurar que el árbol se dibuje correctamente
        public void ActualizarRaiz()
        {
            if (Raiz == null)
                return;

            // Encontrar todas las raíces reales (nodos sin padres)
            var visitados = new ListaEnlazada<NodoFamiliar>();
            var raicesReales = new ListaEnlazada<NodoFamiliar>();
            EncontrarRaicesReales(Raiz, raicesReales, visitados);

            // Si encontramos raíces reales, usar la primera como la raíz principal del árbol
            if (raicesReales.Largo() > 0)
            {
                Raiz = raicesReales.Obtener(0);
            }
        }

    }
}