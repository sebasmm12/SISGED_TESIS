using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using System.Text;
using System.Text.Json;

namespace SISGED.Client.Services.Repositories
{
    public class HttpRepository : IHttpRepository
    {
        private readonly HttpClient _httpClient;

        public HttpRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static JsonSerializerOptions SerializerOptions => new() { PropertyNameCaseInsensitive = true };

        public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
        {
            var httpResponse = await _httpClient.DeleteAsync(url);

            return new HttpResponseWrapper<object>(null, !httpResponse.IsSuccessStatusCode, httpResponse);
        }

        public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
        {
            var httpResponse = await _httpClient.GetAsync(url);

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await DeserializeResponseAsync<T>(httpResponse, SerializerOptions);

                return new HttpResponseWrapper<T>(response, false, httpResponse);
            }

            return new HttpResponseWrapper<T>(default, true, httpResponse);
        }

        public async Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T body)
        {
            var data = ConvertToStringContent(body);

            var httpResponse = await _httpClient.PostAsync(url, data);

            return new HttpResponseWrapper<object>(null, !httpResponse.IsSuccessStatusCode, httpResponse);
        }

        public async Task<HttpResponseWrapper<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body)
        {
            var data = ConvertToStringContent(body);

            var httpResponse = await _httpClient.PostAsync(url, data);

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await DeserializeResponseAsync<TResponse>(httpResponse, SerializerOptions);

                return new HttpResponseWrapper<TResponse>(response, false, httpResponse);
            }

            return new HttpResponseWrapper<TResponse>(default, true, httpResponse);
        }

        public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T body)
        {
            var data = ConvertToStringContent(body);

            var httpResponse = await _httpClient.PutAsync(url, data);

            return new HttpResponseWrapper<object>(null, !httpResponse.IsSuccessStatusCode, httpResponse);
        }

        public async Task<HttpResponseWrapper<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest body)
        {
            var data = ConvertToStringContent(body);

            var httpResponse = await _httpClient.PutAsync(url, data);

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await DeserializeResponseAsync<TResponse>(httpResponse, SerializerOptions);

                return new HttpResponseWrapper<TResponse>(response, false, httpResponse);
            }

            return new HttpResponseWrapper<TResponse>(default, true, httpResponse);
        }

        #region private methods
        private static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage httpResponse, JsonSerializerOptions jsonSerializerOptions)
        {
            var result = await httpResponse.Content.ReadAsStringAsync();

            var response = JsonSerializer.Deserialize<T>(result, jsonSerializerOptions);

            return response!;
        }
        
        private static StringContent ConvertToStringContent<T>(T body)
        {
            var json = JsonSerializer.Serialize(body);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            return data;
        }

        #endregion
    }
}
