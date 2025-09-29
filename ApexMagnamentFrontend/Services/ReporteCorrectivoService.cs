using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using static ApexMagnamentFrontend.DTOs.ReporteCorrectivoDTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ApexMagnamentFrontend.Services
{
    public class ReporteCorrectivoService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _localStore;

        // La URL base específica para los reportes correctivos
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/reportes-correctivos";
        private readonly string _allReportsUrl = "/all"; // Endpoint para obtener todos

        public ReporteCorrectivoService(HttpClient httpClient, ProtectedSessionStorage localStore)
        {
            _httpClient = httpClient;
            _localStore = localStore;
        }

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

        public async Task<List<ReporteCorrectivo>> ObtenerTodosLosReportesCorrectivosAsync()
        {
            try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener reportes correctivos...");

                if (!await ConfigurarTokenAsync())
                    return new List<ReporteCorrectivo>();

                // Construcción de la URL completa
                var url = $"{_baseUrl}{_allReportsUrl}";

                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"📡 Código de respuesta para reportes: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener reportes. La API respondió con un estado no exitoso.");
                    return new List<ReporteCorrectivo>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ JSON recibido para reportes: {json}");

                //  Deserializa DIRECTAMENTE a una lista, 
                // ya que el JSON de la API es un array raíz.
                var reportesList = JsonSerializer.Deserialize<List<ReporteCorrectivo>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Si la deserialización devuelve null (ej., el JSON es "null"), retornamos una lista vacía.
                return reportesList ?? new List<ReporteCorrectivo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener reportes correctivos: {ex.Message}");
                // Si hay cualquier excepción (red, deserialización, etc.), devolvemos una lista vacía.
                return new List<ReporteCorrectivo>();
            }
        }
    }
}