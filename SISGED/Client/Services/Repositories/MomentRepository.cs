using Microsoft.JSInterop;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Services.Repositories
{
    public class MomentRepository : IMomentRepository
    {
        private readonly IJSRuntime _jsRuntime;

        public MomentRepository(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetTimeFromNowAsync(DateTime date)
        {
            string timeFromNow = await _jsRuntime.InvokeAsync<string>("getTimeFromNow", date.ToString("dd/MM/yyyy"));

            return timeFromNow;
        }
    }
}
