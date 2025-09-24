using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;

namespace ApexMagnamentFrontend.Services
{
    public class AutheService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedSessionStorage _localStore;
        private string? _token;
        private bool _isAuthenticated = false;

        public AutheService(ProtectedSessionStorage localStore, HttpClient httpClient)
        {
            _localStore = localStore;
            _httpClient = httpClient;
        }
        public async Task<string> CrearUsuario(CreateUser createUser)
        {
            try
            {
                // Obtener el token actual
                var token = await GetToken();
                if (string.IsNullOrEmpty(token))
                {
                    return "Error: No hay sesión activa. Por favor, inicie sesión.";
                }

                // Agregar el token al header de la solicitud
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("api/personal", createUser);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return null;

                return $"Error {(int)response.StatusCode}: {responseContent}";
            }
            catch (Exception ex)
            {
                return $"Error de conexión: {ex.Message}";
            }
        }

        public async Task<string> Login(UserSession userSesion)
        {
            var response = await _httpClient.PostAsJsonAsync("api/personal/login", userSesion);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<string>();
                return result;
            }
            return null;

        }

        public async Task SetToken(string token)
        {
            _token = token;
            await _localStore.SetAsync("token", token);
        }

        public async Task<string?> GetToken()
        {
            var localStoreResult = await _localStore.GetAsync<string>("token");

            if (string.IsNullOrEmpty(_token))
            {
                if (!localStoreResult.Success || string.IsNullOrEmpty(localStoreResult.Value))
                {
                    _token = null;
                    return null;
                }
                _token = localStoreResult.Value;
            }
            return _token;

        }

        public async Task<bool> IsAuthenticated()
        {
            if (_isAuthenticated)
                return true;

            try
            {
                var token = await GetToken();
                _isAuthenticated = !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
                return _isAuthenticated;
            }
            catch
            {
                return false;
            }
        }

        public bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        public async Task Logout()
        {
            _token = null;
            await _localStore.DeleteAsync("token");

        }
    }
}