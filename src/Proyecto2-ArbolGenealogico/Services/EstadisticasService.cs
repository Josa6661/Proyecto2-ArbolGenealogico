using Proyecto2_ArbolGenealogico.DataStructures;
using System;

namespace Proyecto2_ArbolGenealogico.Services
{
    public class EstadisticasService
    {
        public class ResultadoEstadisticas
        {
            public bool HayDatosSuficientes { get; set; }
            public string MensajeError { get; set; }

            public NodoFamiliar ParLejanoA { get; set; }
            public NodoFamiliar ParLejanoB { get; set; }
            public double DistanciaMaxima { get; set; }

            public NodoFamiliar ParCercanoA { get; set; }
            public NodoFamiliar ParCercanoB { get; set; }
            public double DistanciaMinima { get; set; }

            public double DistanciaPromedio { get; set; }
            public int TotalPares { get; set; }
        }

        public static ResultadoEstadisticas CalcularEstadisticas(ArbolGenealogico arbol)
        {
            var resultado = new ResultadoEstadisticas();

            if (!arbol.TieneRaiz())
            {
                resultado.HayDatosSuficientes = false;
                resultado.MensajeError = "No hay familiares suficientes.";
                return resultado;
            }

            var lista = arbol.ObtenerTodos();
            int n = lista.Largo();

            if (n < 2)
            {
                resultado.HayDatosSuficientes = false;
                resultado.MensajeError = "Se necesitan al menos 2 familiares.";
                return resultado;
            }

            double maxDist = -1;
            double minDist = double.MaxValue;
            double sumaDist = 0;
            int conteo = 0;

            NodoFamiliar lejosA = null, lejosB = null;
            NodoFamiliar cercaA = null, cercaB = null;

            for (int i = 0; i < n; i++)
            {
                var f1 = lista.Obtener(i);

                if (double.IsNaN(f1.Latitud) || double.IsNaN(f1.Longitud))
                    continue;

                for (int j = i + 1; j < n; j++)
                {
                    var f2 = lista.Obtener(j);

                    if (double.IsNaN(f2.Latitud) || double.IsNaN(f2.Longitud))
                        continue;

                    double d = MapaService.CalcularDistanciaHaversine(
                        f1.Latitud, f1.Longitud,
                        f2.Latitud, f2.Longitud
                    );

                    if (double.IsNaN(d) || double.IsInfinity(d))
                        continue;

                    sumaDist += d;
                    conteo++;

                    if (d > maxDist)
                    {
                        maxDist = d;
                        lejosA = f1;
                        lejosB = f2;
                    }

                    if (d < minDist)
                    {
                        minDist = d;
                        cercaA = f1;
                        cercaB = f2;
                    }
                }
            }

            if (conteo == 0)
            {
                resultado.HayDatosSuficientes = false;
                resultado.MensajeError = "No hay suficientes familiares con coordenadas válidas.";
                resultado.DistanciaPromedio = 0;
                return resultado;
            }

            resultado.HayDatosSuficientes = true;
            resultado.ParLejanoA = lejosA;
            resultado.ParLejanoB = lejosB;
            resultado.DistanciaMaxima = maxDist;
            resultado.ParCercanoA = cercaA;
            resultado.ParCercanoB = cercaB;
            resultado.DistanciaMinima = minDist;
            resultado.DistanciaPromedio = conteo > 0 ? sumaDist / conteo : 0;
            resultado.TotalPares = conteo;

            return resultado;
        }
    }
}