using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using static ApexMagnamentFrontend.DTOs.ReporteCorrectivoDTO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text; // Necesario para StringContent

namespace ApexMagnamentFrontend.Services
{
    public class ReporteCorrectivoService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _localStore;

        // La URL base específica para los reportes correctivos
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/reportes-correctivos";
        private readonly string _allReportsUrl = "/all";

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

        //  Obtener todos
        public async Task<List<ReporteCorrectivo>> ObtenerTodosLosReportesCorrectivosAsync()
        {
           try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener reportes correctivos...");

                if (!await ConfigurarTokenAsync())
                    return new List<ReporteCorrectivo>();

                var url = $"{_baseUrl}{_allReportsUrl}";

                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"📡 Código de respuesta para reportes: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener reportes. La API respondió con un estado no exitoso.");
                    return new List<ReporteCorrectivo>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var reportesList = JsonSerializer.Deserialize<List<ReporteCorrectivo>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return reportesList ?? new List<ReporteCorrectivo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener reportes correctivos: {ex.Message}");
                return new List<ReporteCorrectivo>();
            }
        }

        // MÉTODO Crear reporte correctivo
        public async Task<(bool Exito, string Mensaje)> CrearReporteCorrectivoAsync(ReporteCorrectivoCreacionDTO reporteCreacion)
        {
            try
            {
                if (!await ConfigurarTokenAsync())
                    return (false, "Error de autenticación. Inicie sesión nuevamente.");

                Console.WriteLine($"📤 Enviando reporte para solicitud: {reporteCreacion.SolicitudId}");
                Console.WriteLine($"🔧 Tipo mantenimiento: {reporteCreacion.TipoMantenimiento}");

                // ✅ PostAsJsonAsync es correcto para esta API
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, reporteCreacion);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Reporte creado exitosamente. Código: {response.StatusCode}");
                    return (true, "Reporte Correctivo creado exitosamente.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al crear reporte. Código: {response.StatusCode}");
                    Console.WriteLine($"📋 Detalle del error: {errorContent}");

                    // Manejo específico de errores comunes
                    return response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.BadRequest => (false, $"Datos inválidos: {errorContent}"),
                        System.Net.HttpStatusCode.NotFound => (false, "Solicitud no encontrada"),
                        System.Net.HttpStatusCode.Conflict => (false, "Ya existe un reporte para esta solicitud"),
                        System.Net.HttpStatusCode.Forbidden => (false, "Sin permisos para crear reportes"),
                        _ => (false, $"Error {response.StatusCode}: {errorContent}")
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al crear reporte: {ex.Message}");
                return (false, $"Error de conexión: {ex.Message}");
            }
        }
    }
}