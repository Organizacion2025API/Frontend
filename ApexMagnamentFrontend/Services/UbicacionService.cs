using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using ApexMagnamentFrontend.DTOs;
using ApexMagnamentFrontend.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ApexMagnamentFrontend.Services
{
    public class UbicacionService
    {
        private readonly HttpClient _http;
        private readonly ProtectedSessionStorage _localStore;
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/ubicaciones";

        public UbicacionService(HttpClient http, ProtectedSessionStorage localStore)
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

        // Obtener todas las ubicaciones
        public async Task<List<UbicacionDTO>> GetAllAsync()
        {
            try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener ubicaciones...");

                if (!await ConfigurarTokenAsync())
                    return new List<UbicacionDTO>();

                var response = await _http.GetAsync(_baseUrl);
                Console.WriteLine($"📡 Código de respuesta: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener ubicaciones. La API respondió con un estado no exitoso.");
                    return new List<UbicacionDTO>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ JSON recibido: {json}");

                // Deserializar usando PaginatedResponse
                var paged = JsonSerializer.Deserialize<PaginatedResponse<UbicacionDTO>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return paged?.Content ?? new List<UbicacionDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener ubicaciones: {ex.Message}");
                return new List<UbicacionDTO>();
            }
        }

        // Crear ubicación 
        public async Task<bool> CreateAsync(UbicacionDTO nuevaUbicacion)
        {
            try
            {
                // 🔹 Usar tus variables para el token
                if (!await ConfigurarTokenAsync())
                return false;
                // 🔹 Enviar POST a la API
                var response = await _http.PostAsJsonAsync(_baseUrl, nuevaUbicacion);

                // 🔹 Revisar respuesta
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al crear ubicación: {response.StatusCode} - {error}");
                }
                else
                {
                    Console.WriteLine("✅ Ubicación creada correctamente.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al crear ubicación: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(int id, UbicacionDTO ubicacionActualizada)
        {
            try
            {
                // 🔹 Configurar token antes de la solicitud
                if (!await ConfigurarTokenAsync())
                    return false;

                // 🔹 Enviar PUT a la API
                var response = await _http.PutAsJsonAsync($"{_baseUrl}/{id}", ubicacionActualizada);

                // 🔹 Revisar respuesta
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al actualizar ubicación: {response.StatusCode} - {error}");
                }
                else
                {
                    Console.WriteLine("✅ Ubicación actualizada correctamente.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al actualizar ubicación: {ex.Message}");
                return false;
            }
        }
    }
}
