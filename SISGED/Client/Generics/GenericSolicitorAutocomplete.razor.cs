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
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISolicitorRepository SolicitorRepository { get; set; } = default!;

        [Parameter]
        public bool? ExSolicitor { get; set; }
        [Parameter]
        public EventCallback<AutocompletedSolicitorResponse> SolicitorResponse { get; set; } = default!;
        [Parameter]
        public AutocompletedSolicitorResponse? Solicitor { get; set; } = null;
        [Parameter]
        public bool CanShowSolicitorHelper { get; set; } = false;

        private int SolicitorMeasurement => CanShowSolicitorHelper ? 11: 12;

        private async Task GetSolicitorResponseAsync(AutocompletedSolicitorResponse AutocompletedSolicitorResponse)
        {
            Solicitor = AutocompletedSolicitorResponse;
            await SolicitorResponse.InvokeAsync(AutocompletedSolicitorResponse);
        }

        private static string? ConvertToString(AutocompletedSolicitorResponse AutocompletedSolicitorResponse)
        {
            return AutocompletedSolicitorResponse is null ? null : AutocompletedSolicitorResponse.Name + " " + AutocompletedSolicitorResponse.LastName;
        }

        private async Task<IEnumerable<AutocompletedSolicitorResponse>> GetAutocompletedSolicitorAsync(string solicitorName)
        {
            await Task.Delay(100);

            try
            {
                string solicitorQueries = "?";

                var filters = ConvertToFilters(solicitorName);

                if (filters.Count > 0) solicitorQueries += filters.Select(filter => $"{filter.Key}=" +
                $"{System.Web.HttpUtility.UrlEncode(filter.Value.ToString())}").Aggregate((current, next) => $"{current}&{next}");

                var autocompletedSolicitorResponse = await HttpRepository.GetAsync<IEnumerable<AutocompletedSolicitorResponse>>($"api/solicitors/autocomplete{solicitorQueries}");

                if (autocompletedSolicitorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los notarios registrados en el sistema");
                }

                return autocompletedSolicitorResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los notarios registrados en el sistema");
                return new List<AutocompletedSolicitorResponse>();
            }
        }

        private Dictionary<string, object> ConvertToFilters(string solicitorName)
        {
            var solicitorFilter = new SolicitorFilter(solicitorName, ExSolicitor);

            return SolicitorRepository.ConvertToFilters(solicitorFilter);
        }
    }
}