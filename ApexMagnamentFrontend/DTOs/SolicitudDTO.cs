using System.Text.Json.Serialization;

namespace ApexMagnamentFrontend.DTOs
{
    public class SolicitudDTO
    {
        public class Solicitud
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("descripcion")]
            public string? Descripcion { get; set; }

            [JsonPropertyName("estado")]
            public short Estado { get; set; }

            [JsonPropertyName("fechaRegistro")]
            public DateTime? FechaRegistro { get; set; }

            [JsonPropertyName("asignacionEquipoId")]
            public int AsignacionEquipoId { get; set; }

            [JsonPropertyName("personalId")]
            public string PersonalId { get; set; } = string.Empty;
        }
    }
}
