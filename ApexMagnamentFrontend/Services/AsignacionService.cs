using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Text.Json;
namespace ApexMagnamentFrontend.Services
{
    public class AsignacionService
    {
        private readonly HttpClient _http;
        private readonly ProtectedSessionStorage _localStore;
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/asignaciones";
        private string? _token;

       


        public AsignacionService(HttpClient http, ProtectedSessionStorage localStore)
        {
            _http = http;
            _localStore = localStore;
        }

        // Configurar token
        private async Task<bool> ConfigurarTokenAsync()
        {
            var result = await _localStore.GetAsync<string>("token");
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Value);
                return true;
            }
            Console.WriteLine("⚠️ Token no encontrado. La API requiere autenticación.");
            return false;
        }

        // 🔹 Obtener todas asignaciones
        public async Task<List<AsignacionDTO>> GetAllAsync()
        {
            try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener asignaciones...");

                if (!await ConfigurarTokenAsync())
                    return new List<AsignacionDTO>();

                var response = await _http.GetAsync(_baseUrl);
                Console.WriteLine($"📡 Código de respuesta: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener asignaciones. La API respondió con un estado no exitoso.");
                    return new List<AsignacionDTO>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ JSON recibido: {json}");

                // En lugar de deserializar a un objeto paginado, deserializa a una lista directa
                var asignaciones = JsonSerializer.Deserialize<List<AsignacionDTO>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return asignaciones ?? new List<AsignacionDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener asignaciones: {ex.Message}");
                return new List<AsignacionDTO>();
            }
        }

        // 🔹 Crear asignacion
        public async Task<bool> CreateAsync(AsignacionDTO nuevo)
        {
            if (!await ConfigurarTokenAsync()) return false;

            var response = await _http.PostAsJsonAsync(_baseUrl, nuevo);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al crear asignacion: {response.StatusCode} - {error}");
            }
            else
            {
                Console.WriteLine("✅ asignacion creada correctamente.");
            }

            return response.IsSuccessStatusCode;
        }

        // 🔹 Eliminar asignacion
        public async Task<bool> DeleteAsync(int id)
        {
            if (!await ConfigurarTokenAsync()) return false;

            var response = await _http.DeleteAsync($"{_baseUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al eliminar la asignacion: {response.StatusCode} - {error}");
            }
            else
            {
                Console.WriteLine("✅ asignacion eliminada correctamente.");
            }

            return response.IsSuccessStatusCode;
        }

    }
}
