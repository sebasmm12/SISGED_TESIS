using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Client.Generics
{
    public partial class GenericSolicitorAutocomplete
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISolicitorRepository SolicitorRepository { get; set; } = default!;

        [Parameter]
        public bool? ExSolicitor { get; set; }
        [Parameter]
        public SolicitorInfoResponse SelectedSolicitor { get; set; } = default!;

        private string? ConvertToString(SolicitorInfoResponse solicitorInfoResponse)
        {
            return solicitorInfoResponse is null ? null : solicitorInfoResponse.Name + " " + solicitorInfoResponse.LastName;
        }

        private async Task<IEnumerable<SolicitorInfoResponse>> GetAutocompletedSolicitorAsync(string solicitorName)
        {
            await Task.Delay(100);

            try
            {
                string solicitorQueries = "?";

                var filters = ConvertToFilters(solicitorName);

                if (filters.Count > 0) solicitorQueries += filters.Select(filter => $"{filter.Key}=" +
                $"{System.Web.HttpUtility.UrlEncode(filter.Value.ToString())}").Aggregate((current, next) => $"{current}&{next}");

                var autocompletedSolicitorResponse = await HttpRepository.GetAsync<IEnumerable<SolicitorInfoResponse>>($"api/solicitors/autocomplete{solicitorQueries}");

                if (autocompletedSolicitorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los notarios registrados en el sistema");
                }

                await JSRuntime.InvokeVoidAsync("console.log", autocompletedSolicitorResponse.Response);

                return autocompletedSolicitorResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los notarios registrados en el sistema");
                return new List<SolicitorInfoResponse>();
            }
        }

        private Dictionary<string, object> ConvertToFilters(string solicitorName)
        {
            var solicitorFilter = new SolicitorFilter(solicitorName, ExSolicitor);

            return SolicitorRepository.ConvertToFilters(solicitorFilter);
        }
    }
}