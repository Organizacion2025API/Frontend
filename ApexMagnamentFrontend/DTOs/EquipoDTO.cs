namespace ApexMagnamentFrontend.DTOs
{
    public class EquipoDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int Garantia { get; set; }
        public string? Img { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string NSerie { get; set; } = string.Empty;
        public int? CategoriaId { get; set; }
        public int? UbicacionId { get; set; }
    }

    public class CrearEquipoDTO
    {
        public string? Nombre { get; set; }
        public string? Modelo { get; set; }
        public string? NSerie { get; set; }
        public string? Descripcion { get; set; }
        public int Garantia { get; set; }
        public string? Img { get; set; }
        public int CategoriaId { get; set; }
        public int UbicacionId { get; set; }
    }

    public class EditarEquipoDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Modelo { get; set; }
        public string? NSerie { get; set; }
        public string? Descripcion { get; set; }
        public int Garantia { get; set; }
        public string? Img { get; set; }
        public int? CategoriaId { get; set; }
        public int? UbicacionId { get; set; }
    }


}
