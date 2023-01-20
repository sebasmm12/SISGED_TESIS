using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using SISGED.Client;
using SISGED.Client.Generics;
using SISGED.Client.Shared;
using SISGED.Client.Helpers;
using SISGED.Client.Components.WorkEnvironments;
using SISGED.Client.Components.SolicitorDossier;
using SISGED.Shared.Models.Responses.Tray;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.PublicDeed;
using SISGED.Shared.Models.Responses.User;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using SISGED.Shared.Models.Responses.Solicitor;
using SISGED.Shared.Models.Responses.DocumentType;
using SISGED.Shared.DTOs;
using MudBlazor;
using MudExtensions;
using MudExtensions.Enums;
using SISGED.Client.Services.Contracts;
using System.Text.Json;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class ResolutionGenerator
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;

        private ResolutionContentDTO resolutionContent = default!;
        private ResolutionDTO resolution = default!;
        private bool canShow = false;

        protected override async Task OnInitializedAsync()
        {
            resolutionContent = JsonSerializer.Deserialize<ResolutionContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content))!;

            await GetResolutionInfoAsync();

            canShow = true;
        }

        private async Task GetResolutionInfoAsync()
        {
            var solicitorTask = GetSolicitorAsync(resolutionContent.SolicitorId);
            var documentTypeTask = GetDocumentTypeAsync(resolutionContent.Sanction);

            await Task.WhenAll(solicitorTask, documentTypeTask);

            var solicitor = await solicitorTask;
            var documentType = await documentTypeTask;

            resolution = new(DocumentGenerator.Dossier.Client!, solicitor, documentType);
        }

        private async Task<AutocompletedSolicitorResponse> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitorResponse = await HttpRepository.GetAsync<AutocompletedSolicitorResponse>($"api/solicitors/{solicitorId}");

                if (solicitorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del notario");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener la información del notario");
                return new();
            }
        }

        private async Task<DocumentTypeInfoResponse> GetDocumentTypeAsync(string documentTypeId)
        {
            try
            {
                var documentTypesResponse = await HttpRepository.GetAsync<DocumentTypeInfoResponse>($"api/documentTypes/{documentTypeId}");

                if (documentTypesResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de sanciones del sistema");
                }

                return documentTypesResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de sanciones del sistema");
                return new();
            }
        }
    }
}