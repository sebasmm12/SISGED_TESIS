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
using SISGED.Shared.Models.Responses.Tray;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.PublicDeed;
using SISGED.Shared.Models.Responses.User;
using SISGED.Shared.Models.Responses.Document.UserRequest;
using MudBlazor;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Requests.Documents;
using Newtonsoft.Json;
using SISGED.Shared.Validators;
using SISGED.Shared.DTOs;
using AutoMapper;
using MudExtensions;
using MudExtensions.Utilities;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Responses.Solicitor;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class ResolutionRegister
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public ResolutionRegisterValidator ResolutionRegisterValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository dialogContentRepository { get; set; } = default!;

        private MudForm? requestForm = default!;
        private MudStepper? requestStepper;

        //Variables de sesion
        [CascadingParameter(Name = "WorkEnvironment")]
        public WorkEnvironment WorkEnvironment { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")] protected SessionAccountResponse SessionAccount { get; set; }

        //Datos del formulario
        private ResolutionRegisterDTO resolutionRegister = new ResolutionRegisterDTO();
        
        private List<MediaRegisterDTO> annexes = new();
        List<string> names = new List<string>();
        private bool pageLoading = true;
        private string dossierId = default!;

        String typeDocument = "Resolucion";

        protected override async Task OnInitializedAsync()
        {
            resolutionRegister.Participants = new List<Participant>() { new Participant() { Index = 0, Name = "" } };
            await GetUserRequestInformationAsync();

            pageLoading = false;
        }

        private void addParticipant()
        {
            resolutionRegister.Participants.Add(new Participant() { Index = (resolutionRegister.Participants.Count) });
            StateHasChanged();
        }

        private void removeParticipant(int index)
        {
            resolutionRegister.Participants.RemoveAt(index);
            StateHasChanged();
        }

        private DossierWrapper GetDocumentRegister()
        {
            var resolutionContent = Mapper.Map<ResolutionResponseContent>(resolutionRegister);
            var resolution = new ResolutionResponse(resolutionContent, annexes);

            var documentRegister = new DossierWrapper(dossierId, resolution);

            return documentRegister;
        }

        private async Task<Resolution?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su resolución, por favor espere...";

            var toRegister = () => RegisterResolutionAsync(documentRegister);

            return await dialogContentRepository.ShowLoadingDialogAsync(toRegister, dialogTitle);

        }

        private async Task RegisterResolutionAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredResolution = await ShowLoadingDialogAsync(documentRegister);

            if (registeredResolution is null) return;

            await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar la resolución de manera satisfactoria");

            UpdateRegisteredDocument(registeredResolution);
        }

        private void UpdateRegisteredDocument(Resolution resolution)
        {
            var inputItem = WorkEnvironment.workPlaceItems.FirstOrDefault(workItem => workItem.OriginPlace == "inputs");

            ProcessWorkItemInfo(inputItem!, resolution);

            WorkEnvironment.UpdateRegisteredDocument(inputItem!);

        }

        private void ProcessWorkItemInfo(Item item, Resolution resolution)
        {
            if (item.Value is not DossierTrayResponse dossierTray) return;

            var documentResponse = Mapper.Map<DocumentResponse>(resolution);

            dossierTray.DocumentObjects!.Add(documentResponse);
            dossierTray.Document = documentResponse;
            dossierTray.Type = typeDocument;

            Mapper.Map(dossierTray, item);
        }

        private async Task<Resolution?> RegisterResolutionAsync(DossierWrapper documentRegister)
        {
            try
            {
                var resolutionResponse = await httpRepository.PostAsync<DossierWrapper, Resolution>("api/documents/resolutions", documentRegister);

                if (resolutionResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la resolución");
                }

                return resolutionResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar la resolución");
                return null;
            }
        }



        private async Task GetUserRequestInformationAsync()
        {
            var userTray = WorkEnvironment.workPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            //resolutionRegister.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;

            //var documentContent = JsonSerializer.Deserialize<InitialRequestContentDTO>(JsonSerializer.Serialize(dossierTray!.Document!.Content));

            //resolutionRegister.Solicitor = await GetSolicitorAsync(documentContent!.SolicitorId);
            dossierId = dossierTray!.DossierId;
        }

        private static StepperLocalizedStrings GetRegisterLocalizedStrings()
        {
            return new() { Completed = "Completado", Finish = "Registrar", Next = "Siguiente", Previous = "Anterior" };
        }

        private bool CheckRegisterAsync()
        {
            if (requestStepper!.GetActiveIndex() != 2) return false;

            requestForm!.Validate().GetAwaiter().GetResult();

            if (!requestForm!.IsValid)
            {
                swalFireRepository.ShowErrorSwalFireAsync("No se puede registrar la resolución, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterResolutionAsync().GetAwaiter();

            return false;
        }
    }
}