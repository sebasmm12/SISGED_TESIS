namespace SISGED.Client.Helpers
{
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(T? response, bool error, HttpResponseMessage httpResponseMessage)
        {
            Error = error;
            Response = response;
            HttpResponseMessage = httpResponseMessage;
        }

        public async Task<string> GetBodyAsync()
        {
            return await HttpResponseMessage.Content.ReadAsStringAsync();
        }

        public bool Error { get; set; }
        public T? Response { get; set; } = default!;
        public HttpResponseMessage HttpResponseMessage { get; set; } = default!;
    }
}
