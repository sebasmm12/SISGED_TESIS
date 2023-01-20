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
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Entities;

namespace SISGED.Client.Components.Documents.Generators
{
    public partial class DisciplinaryOpennessGenerator
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [CascadingParameter(Name = "DocumentGenerator")]
        public DocumentGenerator DocumentGenerator { get; set; } = default!;

        private DisciplinaryOpennessContentDTO disciplinaryOpennessContent = default!;
        private DisciplinaryOpennessDTO disciplinaryOpenness = default!;
        private bool canShow = false;

        protected override async Task OnInitializedAsync()
        {
            disciplinaryOpennessContent = JsonSerializer.Deserialize<DisciplinaryOpennessContentDTO>(JsonSerializer.Serialize(DocumentGenerator.Document.Content))!;

            await GetDisciplinaryOpennessInfoAsync();

            canShow = true;
        }

        private async Task GetDisciplinaryOpennessInfoAsync()
        {
            var solicitorTask = GetSolicitorAsync(disciplinaryOpennessContent.SolicitorId);
            var prosecutorTask = GetProsecutorInformationAsync(disciplinaryOpennessContent.ProsecutorId);

            await Task.WhenAll(solicitorTask, prosecutorTask);

            var solicitor = await solicitorTask;
            var prosecutor = await prosecutorTask;

            disciplinaryOpenness = new(DocumentGenerator.Dossier.Client!, solicitor, prosecutor);
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

        private async Task<ProsecutorUserInfoResponse> GetProsecutorInformationAsync(string prosecutorId)
        {
            try
            {
                var prosecutorResponse = await HttpRepository.GetAsync<List<ProsecutorUserInfoResponse>>($"api/users/prosecutors");

                if (prosecutorResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los fiscales del sistema");
                }

                return prosecutorResponse.Response!.FirstOrDefault(l => l.Id == prosecutorId)!;
            }
            catch (Exception)
            {

                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los fiscales del sistema");
                return new();
            }
        }
    }
}