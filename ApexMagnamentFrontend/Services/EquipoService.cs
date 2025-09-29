using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ApexMagnamentFrontend.Services
{
    public class EquipoService
    {
        private readonly HttpClient _http;
        private readonly ProtectedSessionStorage _localStore;
        private readonly string _baseUrl = "https://gateway-api-dfbk.onrender.com/ApiAdministracion/api/equipos";

        public EquipoService(HttpClient http, ProtectedSessionStorage localStore)
        {
            _http = http;
            _localStore = localStore;
        }

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

        // 🔹 Obtener todos los equipos
        public async Task<List<EquipoDTO>> GetAllAsync()
        {
            try
            {
                Console.WriteLine("📡 Intentando conectar con la API para obtener ubicaciones...");

                if (!await ConfigurarTokenAsync())
                    return new List<EquipoDTO>();

                var response = await _http.GetAsync(_baseUrl);
                Console.WriteLine($"📡 Código de respuesta: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Error al obtener equipos. La API respondió con un estado no exitoso.");
                    return new List<EquipoDTO>();
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ JSON recibido: {json}");

                // Deserializar usando PaginatedResponse
                var paged = JsonSerializer.Deserialize<PaginatedResponse<EquipoDTO>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return paged?.Content ?? new List<EquipoDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado al obtener equipos: {ex.Message}");
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
        public async Task<bool> CreateAsync(CrearEquipoDTO nuevoEquipo)
        {
            try
            {
                if (!await ConfigurarTokenAsync())
                    return false;

                // 🔧 CREAR FORMDATA EN LUGAR DE JSON
                var formData = new MultipartFormDataContent();

                // Agregar todos los campos como FormData
                formData.Add(new StringContent(nuevoEquipo.NSerie ?? ""), "nserie");
                formData.Add(new StringContent(nuevoEquipo.Nombre ?? ""), "nombre");
                formData.Add(new StringContent(nuevoEquipo.Modelo ?? ""), "modelo");
                formData.Add(new StringContent(nuevoEquipo.Descripcion ?? ""), "descripcion");
                formData.Add(new StringContent(nuevoEquipo.Garantia.ToString()), "garantia");
                formData.Add(new StringContent(nuevoEquipo.CategoriaId.ToString()), "categoriaId");
                formData.Add(new StringContent(nuevoEquipo.UbicacionId.ToString()), "ubicacionId");
                Console.WriteLine($"📤 Enviando FormData a: {_baseUrl}");

                // 🔧 USAR PostAsync CON FORMDATA
                var response = await _http.PostAsync(_baseUrl, formData);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error: {response.StatusCode} - {error}");
                    return false;
                }

                Console.WriteLine("✅ Equipo creado correctamente.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción: {ex.Message}");
                return false;
            }
        }


        // 🔹 Editar equipo
        public async Task<bool> UpdateAsync(int id, EditarEquipoDTO equipoActualizado)
        {
            try
            {
                if (!await ConfigurarTokenAsync())
                    return false;

                using var form = new MultipartFormDataContent();

                // ✅ Agregar campos en camelCase
                form.Add(new StringContent(equipoActualizado.Nombre ?? ""), "nombre");
                form.Add(new StringContent(equipoActualizado.Modelo ?? ""), "modelo");
                form.Add(new StringContent(equipoActualizado.NSerie ?? ""), "nserie");
                form.Add(new StringContent(equipoActualizado.Descripcion ?? ""), "descripcion");
                form.Add(new StringContent(equipoActualizado.Garantia.ToString()), "garantia");
                form.Add(new StringContent(equipoActualizado.CategoriaId?.ToString() ?? ""), "categoriaId");
                form.Add(new StringContent(equipoActualizado.UbicacionId?.ToString() ?? ""), "ubicacionId");

                // Agregar imagen si existe
                if (!string.IsNullOrEmpty(equipoActualizado.Img))
                {
                    var bytes = File.ReadAllBytes(equipoActualizado.Img);
                    var byteContent = new ByteArrayContent(bytes);
                    form.Add(byteContent, "imagen", Path.GetFileName(equipoActualizado.Img));
                }

                Console.WriteLine($"📤 Enviando PUT FormData a: {_baseUrl}/{id}");

                var response = await _http.PutAsync($"{_baseUrl}/{id}", form);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al actualizar equipo: {response.StatusCode} - {error}");
                }
                else
                {
                    Console.WriteLine("✅ Equipo actualizado correctamente.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al actualizar equipo: {ex.Message}");
                return false;
            }
        }

        // 🔹 Eliminar equipo
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (!await ConfigurarTokenAsync())
                    return false;

                Console.WriteLine($"🗑 Enviando DELETE a: {_baseUrl}/{id}");

                var response = await _http.DeleteAsync($"{_baseUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al eliminar equipo: {response.StatusCode} - {error}");

                    // Manejo específico del error 409 (Conflict)
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        Console.WriteLine("⚠ No se puede eliminar: El equipo tiene asignaciones activas");
                    }
                }
                else
                {
                    Console.WriteLine("✅ Equipo eliminado correctamente.");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al eliminar equipo: {ex.Message}");
                return false;
            }
        }
    }
}


