using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using static ApexMagnamentFrontend.DTOs.AsignacionDTO;
using static ApexMagnamentFrontend.DTOs.ReporteCorrectivoDTO;
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


        //var response = await _http.PostAsJsonAsync(_baseUrl, nuevo);
        /* public async Task<bool> CreateAsync(CrearAsignacionEquipoDTO nuevaAsignacion)
         {
             try
             {
                 if (!await ConfigurarTokenAsync())
                     return false;

                 // 🔧 CREAR FORMDATA EN LUGAR DE JSON
                 var formData = new MultipartFormDataContent();

                 // Agregar todos los campos como FormData
                 formData.Add(new StringContent(nuevaAsignacion.EquipoId.ToString()), "equipoId");
                 formData.Add(new StringContent(nuevaAsignacion.PersonalId.ToString()), "personalId");
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
         }*/

        public async Task<(bool Exito, string Mensaje)> CrearReporteCorrectivoAsync(CrearAsignacionEquipoDTO asignacionCreacion)
        {
            try
            {
                if (!await ConfigurarTokenAsync())
                    return (false, "Error de autenticación. Inicie sesión nuevamente.");

                Console.WriteLine($"📤 Enviando reporte para solicitud: {asignacionCreacion.EquipoId}");
                Console.WriteLine($"🔧 Tipo mantenimiento: {asignacionCreacion.PersonalId}");

                // ✅ PostAsJsonAsync es correcto para esta API
                var response = await _http.PostAsJsonAsync(_baseUrl, asignacionCreacion);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ asignacion creada exitosamente. Código: {response.StatusCode}");
                    return (true, "asignacion creado exitosamente.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al crear asignacion. Código: {response.StatusCode}");
                    Console.WriteLine($"📋 Detalle del error: {errorContent}");

                    // Manejo específico de errores comunes
                    return response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.BadRequest => (false, $"Datos inválidos: {errorContent}"),
                        System.Net.HttpStatusCode.NotFound => (false, "asignacion no encontrada"),
                        System.Net.HttpStatusCode.Conflict => (false, "Ya existe una asignacion para esta solicitud"),
                        System.Net.HttpStatusCode.Forbidden => (false, "Sin permisos para crear asignacion"),
                        _ => (false, $"Error {response.StatusCode}: {errorContent}")
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al crear asigmacion: {ex.Message}");
                return (false, $"Error de conexión: {ex.Message}");
            }

        }


        // 🔹 Eliminar asignacion
        public async Task<string> DeleteAsignacionAsync(AsignacionDTO asignacion)
        {
            try
            {
                // Asegura que tienes el token antes de hacer la solicitud.
                if (!await ConfigurarTokenAsync())
                {
                    return "Error: No hay sesión activa. Por favor, inicie sesión.";
                }

                // Realiza la solicitud DELETE a la API, usando el Id del usuario.
                var response = await _http.DeleteAsync($"{_baseUrl}/{asignacion.Id}");

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // La eliminación fue exitosa.
                    return null;
                }

                // Retorna el mensaje de error si la solicitud no fue exitosa.
                return $"Error {(int)response.StatusCode}: {responseContent}";
            }
            catch (Exception ex)
            {
                // Captura cualquier excepción de red o de otro tipo.
                return $"Error de conexión: {ex.Message}";
            }
        }
    }
}
