using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;
using static ApexMagnamentFrontend.DTOs.SolicitudDTO;

namespace ApexMagnamentFrontend.Services
{
    public class SolicitudService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _localStore;

        // La URL base para las solicitudes
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/solicitudes"; 
        private readonly string _allSolicitudesUrl = "/all"; // /api/solicitudes/all

        public SolicitudService(HttpClient httpClient, ProtectedSessionStorage localStore)
        {
            _httpClient = httpClient;
            _localStore = localStore;
        }

        // Método auxiliar para la configuración del token
        private async Task<bool> ConfigurarTokenAsync()
        {
            var result = await _localStore.GetAsync<string>("token");
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Value);
                return true;
            }
            Console.WriteLine("⚠️ Token no encontrado. La API requiere autenticación.");
            return false;
        }

       
        public async Task<List<Solicitud>> ObtenerTodasLasSolicitudesAsync() 
        {
            try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener solicitudes...");

                if (!await ConfigurarTokenAsync())
                    return new List<Solicitud>();

                var url = $"{_baseUrl}{_allSolicitudesUrl}";

                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"📡 Código de respuesta para solicitudes: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener solicitudes. La API respondió con un estado no exitoso.");
                    return new List<Solicitud>();
                }

                var json = await response.Content.ReadAsStringAsync();

                // Deserializar a List<Solicitud>
                var solicitudesList = JsonSerializer.Deserialize<List<Solicitud>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return solicitudesList ?? new List<Solicitud>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener solicitudes: {ex.Message}");
                return new List<Solicitud>();
            }
        }
    }
}