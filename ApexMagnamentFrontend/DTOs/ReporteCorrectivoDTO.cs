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
            public int TipoMantenimiento { get; set; }
            public int Estado { get; set; }
            public DateTime? FechaCreacion { get; set; }
            public string? NombrePersonal { get; set; }
        }
    }
}

