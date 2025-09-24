using ApexMagnamentFrontend.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;

namespace ApexMagnamentFrontend.Services
{
    public class AutheService
    {

        private readonly ProtectedSessionStorage _localStore;
        private readonly HttpClient _httpClient;
        private string? _token;

        public AutheService(ProtectedSessionStorage localStore, HttpClient httpClient)
        {
            _localStore = localStore;
            _httpClient = httpClient;
        }
        public async Task<bool> CrearUsuario(CreateUser createUser)
        {
            var response = await _httpClient.PostAsJsonAsync("api/personal/", createUser);

            // Devuelve true si el código de estado es 2xx
            return response.IsSuccessStatusCode;
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
            var token = await GetToken();

            return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);

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
