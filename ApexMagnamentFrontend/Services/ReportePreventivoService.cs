using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using static ApexMagnamentFrontend.DTOs.ReportePreventivoDTO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text; // Necesario para StringContent

namespace ApexMagnamentFrontend.Services
{
    public class ReportePreventivoService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _localStore;

        // La URL base específica para los reportes correctivos
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/reportes-preventivos";

        public ReportePreventivoService(HttpClient httpClient, ProtectedSessionStorage localStore)
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

        //  Obtener todos
        public async Task<List<ReportePreventivo>> ObtenerTodosLosReportesPreventivosAsync()
        {
            try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener reportes correctivos...");

                if (!await ConfigurarTokenAsync())
                    return new List<ReportePreventivo>();

                var url = _baseUrl;

                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"📡 Código de respuesta para reportes: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener reportes. La API respondió con un estado no exitoso.");
                    return new List<ReportePreventivo>();
                }
                var json = await response.Content.ReadAsStringAsync();
                var pagedResponse = JsonSerializer.Deserialize<PaginatedResponse<ReportePreventivo>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return pagedResponse?.Content ?? new List<ReportePreventivo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener reportes correctivos: {ex.Message}");
                return new List<ReportePreventivo>();
            }
        }


    }
}

