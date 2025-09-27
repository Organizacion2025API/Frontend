using System.Net.Http.Json;
using System.Text.Json;
using ApexMagnamentFrontend.DTOs;

namespace ApexMagnamentFrontend.Services
{
    public class EquipoService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/equipos";
        private string? _token;

        public EquipoService(HttpClient http)
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

        // 🔹 Obtener todos los equipos
        public async Task<List<EquipoDTO>> GetAllAsync()
        {
            try
            {
                if (!await ConfigurarTokenAsync()) return new List<EquipoDTO>();

                var response = await _http.GetAsync(_baseUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Error al obtener equipos: {response.StatusCode}");
                    return new List<EquipoDTO>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var paged = JsonSerializer.Deserialize<PaginatedResponse<EquipoDTO>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return paged?.Content ?? new List<EquipoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado: {ex.Message}");
                return new List<EquipoDTO>();
            }
        }

        // 🔹 Obtener un equipo por ID
        public async Task<EquipoDTO?> GetByIdAsync(int id)
        {
            if (!await ConfigurarTokenAsync()) return null;

            var response = await _http.GetAsync($"{_baseUrl}/{id}");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<EquipoDTO>();
        }

        // 🔹 Crear equipo
        public async Task<bool> CreateAsync(CrearEquipoDTO nuevo)
        {
            if (!await ConfigurarTokenAsync()) return false;

            var response = await _http.PostAsJsonAsync(_baseUrl, nuevo);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al crear equipo: {response.StatusCode} - {error}");
            }
            else
            {
                Console.WriteLine("✅ Equipo creado correctamente.");
            }

            return response.IsSuccessStatusCode;
        }

        // 🔹 Editar equipo
        public async Task<bool> UpdateAsync(int id, EditarEquipoDTO actualizado)
        {
            if (!await ConfigurarTokenAsync()) return false;

            var response = await _http.PutAsJsonAsync($"{_baseUrl}/{id}", actualizado);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al editar equipo: {response.StatusCode} - {error}");
            }
            else
            {
                Console.WriteLine("✅ Equipo editado correctamente.");
            }

            return response.IsSuccessStatusCode;
        }

        // 🔹 Eliminar equipo
        public async Task<bool> DeleteAsync(int id)
        {
            if (!await ConfigurarTokenAsync()) return false;

            var response = await _http.DeleteAsync($"{_baseUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al eliminar equipo: {response.StatusCode} - {error}");
            }
            else
            {
                Console.WriteLine("✅ Equipo eliminado correctamente.");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
