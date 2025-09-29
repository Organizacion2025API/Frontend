namespace ApexMagnamentFrontend.DTOs
{
    public class ReporteCorrectivoDTO
    {
        // Clase para representar un Reporte Correctivo obtenido de la API
        public class ReporteCorrectivo
        {
            public int Id { get; set; }
            public int SolicitudId { get; set; }
            public string? Observacion { get; set; }
            public short TipoMantenimiento { get; set; }    // ✅ Corregido a short
            public short Estado { get; set; }               // ✅ Corregido a short
            public DateTime? FechaCreacion { get; set; }
            public string? NombrePersonal { get; set; }
        }

        // Datos para crear un reporte (este ya está correcto)
        public class ReporteCorrectivoCreacionDTO
        {
            public int SolicitudId { get; set; }
            public string? Observacion { get; set; }
            public short TipoMantenimiento { get; set; }
        }
    }
}