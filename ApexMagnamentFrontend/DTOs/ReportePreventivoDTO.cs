namespace ApexMagnamentFrontend.DTOs
{
    public class ReportePreventivoDTO
    {
        public class ReportePreventivo
        {

            public int Id { get; set; }
            public int CalendarioPreventivoId { get; set; }
            public string? observacion { get; set; }
            public short tipoMantenimiento { get; set; }    // ✅ Corregido a short
            public short estado { get; set; }               // ✅ Corregido a short
            public DateTime? fechaAtencion { get; set; }
            public string? NombrePersonal { get; set; }
            public int equipoId { get; set; }

        }


        public class ReportePreventivoCreacionDTO
        {

            public int Id { get; set; }
            public int CalendarioPreventivoId { get; set; }
            public string? observacion { get; set; }
            public short tipoMantenimiento { get; set; }    // ✅ Corregido a short
           

        }

    }
}
