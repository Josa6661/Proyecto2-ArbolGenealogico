namespace Proyecto2_ArbolGenealogico.BusinessLogic
{
    public static class CalculadoraDistancias
    {
        private const double RadioTierraKm = 6371.0;

        public static double CalcularDistanciaHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = GradosARadianes(lat2 - lat1);
            double dLon = GradosARadianes(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                      Math.Cos(GradosARadianes(lat1)) * Math.Cos(GradosARadianes(lat2)) *
                      Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return RadioTierraKm * c;
        }

        private static double GradosARadianes(double grados)
        {
            return grados * Math.PI / 180.0;
        }
    }
}