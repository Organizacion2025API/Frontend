namespace ApexMagnamentFrontend.DTOs
{
    public class AsignacionDTO
    {
        public int Id { get; set; }

        public int PersonalId { get; set; }
        public int EquipoId { get; set; }
        public string equipoNombre { get; set; }
        public string equipoDescripcion { get; set; }
        public string equipoModelo{ get; set; }

        public string equipoNserie { get; set; }

        public class CrearAsignacionEquipoDTO
        {
            public int PersonalId { get; set; }
            public int EquipoId { get; set; }
            // FechaAsignacion se auto-genera en el servidor normalmente
        }
    }

    
}
