using ApexMagnamentFrontend.DTOs;
using ApexMagnamentFrontend.Components.Pages;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace ApexMagnamentFrontend.Services
{
    public class CategoriaService
    {
        private readonly HttpClient _httpClient;
        private readonly AutheService _authService;
        private readonly ProtectedSessionStorage _localStore;

        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/categorias";

        public CategoriaService(HttpClient httpClient, ProtectedSessionStorage localStore)
        {
            _httpClient = httpClient;
            _localStore = localStore;
        }
        // Configurar token
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


        public async Task<List<Categorias>> ObtenerCategoriasAsync()
        {
            try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener categorias...");

                if (!await ConfigurarTokenAsync())
                    return new List<Categorias>();

                var response = await _httpClient.GetAsync(_baseUrl);
                Console.WriteLine($"📡 Código de respuesta: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener categorias. La API respondió con un estado no exitoso.");
                    return new List<Categorias>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ JSON recibido: {json}");

                // Deserializar usando PaginatedResponse
                var paged = JsonSerializer.Deserialize<PaginatedResponse<Categorias>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return paged?.Content ?? new List<Categorias>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener categorias: {ex.Message}");
                return new List<Categorias>();
            }
        }

        public async Task<bool> CrearCategoriaAsync(CrearCategoria categoria)
        {
            try
            {
                // 🔹 Usar tus variables para el token
                if (!await ConfigurarTokenAsync())
                    return false;
                // 🔹 Enviar POST a la API
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, categoria);

                // 🔹 Revisar respuesta
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al crear categorias: {response.StatusCode} - {error}");
                }
                else
                {
                    Console.WriteLine("✅ Categoria creada correctamente.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al crear categoria: {ex.Message}");
                return false;
            }
                        
        }     

    }
}
