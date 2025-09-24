using System.Text.Json.Serialization;

namespace ApexMagnamentFrontend.DTOs
{
    public class UserSession
    {

        public string User { get; set; } = null;
        public string Password { get; set; } = null;


    }

    public class CreateUser
    {
        [JsonPropertyName("nombre")]
        public string nombre { get; set; } = null!;

        [JsonPropertyName("apellido")]
        public string apellido { get; set; } = null!;

        [JsonPropertyName("Telefono")]
        public string? telefono { get; set; }

        [JsonPropertyName("correo")]
        public string correo { get; set; } = null!;

        [JsonPropertyName("user")]
        public string? user { get; set; }

        [JsonPropertyName("password")]
        public string? password { get; set; }

        [JsonPropertyName("status")]
        public int status { get; set; }

        [JsonPropertyName("fechaIngreso")]
        public DateTime? fechaIngreso { get; set; }

        [JsonPropertyName("fechaSession")]
        public DateTime? fechaSession { get; set; }

        [JsonPropertyName("rolId")]
        public int? rolId { get; set; }
    }
}
