using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;
using SISGED.Shared.Models.Responses.Solicitor;
using System.Text.Json;
using System.Xml.Linq;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class DictumGenerator
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;

        private DictumContentDTO dictumContent = default!;
        private DictumDTO dictum = default!;
        private bool canShow = false;

        protected override async Task OnInitializedAsync()
        {
            dictumContent = JsonSerializer.Deserialize<DictumContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            await GetDictumInfoAsync();

            canShow = true;
        }

        private async Task GetDictumInfoAsync()
        {
            var solicitor = await GetSolicitorAsync(dictumContent.SolicitorId);

            dictum = new(DocumentGenerator.Dossier.Client!, solicitor);
        }

        private async Task<AutocompletedSolicitorResponse> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitorResponse = await HttpRepository.GetAsync<AutocompletedSolicitorResponse>($"api/solicitors/{solicitorId}");

                if (solicitorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new();
            }
        }
    }
}