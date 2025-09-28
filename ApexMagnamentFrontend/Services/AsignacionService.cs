using ApexMagnamentFrontend.DTOs;
using System.Text.Json;
namespace ApexMagnamentFrontend.Services
{
    public class AsignacionService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/asignaciones";
        private string? _token;

        public AsignacionService(HttpClient http)
        {
            _http = http;
        }

        public void SetToken(string token) => _token = token;

        private Task<bool> ConfigurarTokenAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                Console.WriteLine("⚠️ Token no encontrado. La API requiere autenticación.");
                return Task.FromResult(false);
            }

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            return Task.FromResult(true);
        }

        // 🔹 Obtener todas asignaciones
        public async Task<List<AsignacionDTO>> GetAllAsync()
        {
            try
            {
                if (!await ConfigurarTokenAsync()) return new List<AsignacionDTO>();

                var response = await _http.GetAsync(_baseUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error al obtener las asignaciones: {response.StatusCode}");
                    return new List<AsignacionDTO>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var paged = JsonSerializer.Deserialize<PaginatedResponse<AsignacionDTO>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return paged?.Content ?? new List<AsignacionDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado: {ex.Message}");
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
