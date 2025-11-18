# Proyecto 2 - Árbol Genealógico y Grafo Geográfico

Aplicación en C# usando WPF para la gestión y visualización gráfica de un árbol genealógico y sus relaciones geográficas.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![C#](https://img.shields.io/badge/C%23-12.0-239120)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4)
![License](https://img.shields.io/badge/license-Educational-green)

---

## 📋 Descripción

Proyecto académico del curso **CE1103 - Algoritmos y Estructuras de Datos I** que implementa un sistema dual para:
-  Gestionar y visualizar árboles genealógicos familiares
-  Representar geográficamente las residencias de cada miembro
-  Analizar distancias y estadísticas geográficas entre familiares

**Institución:** Tecnológico de Costa Rica  
**Semestre:** II-2025

---

##  Características

- Visualización dinámica del árbol genealógico
- Mapa interactivo con OpenStreetMap
- Cálculo de distancias reales usando fórmula de Haversine
- Estadísticas: pares más cercanos, más lejanos, distancia promedio
- Gestión completa de miembros (agregar, editar, eliminar)
- Soporte para relaciones de cónyuges
- Validaciones robustas de datos
- +10 pruebas unitarias con xUnit

---

## 🚀 Inicio Rápido

### Requisitos
- Windows 10 o superior
- .NET 8.0 SDK
- Visual Studio 2022 (recomendado)

### Ejecutar el proyecto
```bash
# Clonar el repositorio
git clone https://github.com/[tu-usuario]/Proyecto2-ArbolGenealogico.git

# Navegar al directorio
cd Proyecto2-ArbolGenealogico

# Restaurar dependencias
dotnet restore

# Ejecutar la aplicación
dotnet run --project src/Proyecto2-ArbolGenealogico/Proyecto2-ArbolGenealogico.csproj
```

### Ejecutar pruebas
```bash
dotnet test
```

---

## 📂 Estructura del Proyecto
```
Proyecto2-ArbolGenealogico/
├── src/
│   └── Proyecto2-ArbolGenealogico/
│       ├── DataStructures/      # Implementaciones manuales (ListaEnlazada, Pila, Cola)
│       ├── Models/              # NodoFamiliar y modelos de dominio
│       ├── Services/            # MapaService y lógica de negocio
│       ├── Views/               # ArbolView y ventanas WPF
│       ├── Helpers/             # ValidacionHelper
│       └── MainWindow.xaml      # Interfaz principal
├── tests/
│   └── Proyecto2-ArbolGenealogico.Tests/
│       └── DataStructures/      # Pruebas unitarias
└── README.md
```

---

## 🛠️ Tecnologías

| Componente | Tecnología | Versión |
|------------|-----------|---------|
| Lenguaje | C# | .NET 8.0 |
| Framework UI | WPF | .NET 8.0 |
| Mapas | Mapsui | 5.0.0 |
| Testing | xUnit | Última |

---

## 📚 Documentación

Para documentación completa y detallada, visita nuestra [**Wiki**](../../wiki):

- [📐 Diagramas del Sistema](https://github.com/Josa6661/Proyecto2-ArbolGenealogico/wiki/Diagrama-del-Sistema) - Diagramas UML y estructura del sistema
- [🎓 Aprendizajes](https://github.com/Josa6661/Proyecto2-ArbolGenealogico/wiki/Aprendizajes) - Reflexiones técnicas y de equipo
- [📖 Manual de Usuario] - Guía paso a paso(por agregar) 

---

## 👥 Equipo de Desarrollo

- **[Camila Navarro Valverde]** - [@cam0107](https://github.com/cam0107)
- **[Josafat Solano Quirós]** - [@Josa6661](https://github.com/Josa6661)
- **[Ricky Wu Yan]** - [@RckyWu](https://github.com/RckyWu)

---

## 🧪 Pruebas

El proyecto incluye pruebas unitarias completas que validan:
- ✅ Estructuras de datos personalizadas
- ✅ Operaciones del árbol genealógico
- ✅ Funcionalidad del grafo geográfico
- ✅ Cálculos de distancias
- ✅ Validaciones de entrada

---

## 📄 Licencia

Este proyecto es de código abierto con fines educativos para el Tecnológico de Costa Rica.

---

##  Agradecimientos

- Profesor del curso CE1103: Leonardo Andres Araya Martinez
- Comunidad de [Mapsui](https://github.com/Mapsui/Mapsui)
- [OpenStreetMap](https://www.openstreetmap.org/) por los datos cartográficos

---
