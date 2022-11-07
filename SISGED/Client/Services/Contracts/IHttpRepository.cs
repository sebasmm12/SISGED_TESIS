using SISGED.Client.Helpers;

namespace SISGED.Client.Services.Contracts
{
    public interface IHttpRepository
    {
        Task<HttpResponseWrapper<object>> DeleteAsync(string url);
        Task<HttpResponseWrapper<T>> GetAsync<T>(string url);
        Task<HttpResponseWrapper<object>> PostAsync<T>(string url, T body);
        Task<HttpResponseWrapper<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest body);
        Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T body);
    }
}
