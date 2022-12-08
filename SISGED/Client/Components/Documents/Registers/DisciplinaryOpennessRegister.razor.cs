using Microsoft.AspNetCore.Components;
using SISGED.Client.Helpers;
using SISGED.Shared.Models.Responses.DossierTray;
using SISGED.Shared.Models.Responses.User;
using MudBlazor;
using SISGED.Shared.Models.Responses.Document;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using System.Text.Json;
using SISGED.Shared.Validators;
using SISGED.Shared.DTOs;
using AutoMapper;
using SISGED.Shared.Models.Responses.Solicitor;
using MudExtensions;
using MudExtensions.Utilities;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class DisciplinaryOpennessRegister
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public DisciplinaryOpennessRegisterValidator disciplinaryOpennessRegisterValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;
        [Inject]
        public IDialogContentRepository dialogContentRepository { get; set; } = default!;

        private MudForm? requestForm = default!;
        private MudStepper? requestStepper;

        //Variables de sesion
        [CascadingParameter(Name = "WorkPlaceItems")]
        public List<Item> WorkPlaceItems { get; set; } = default!;
        [CascadingParameter(Name = "SessionAccount")] protected SessionAccountResponse SessionAccount { get; set; }
        //Datos del formulario
        [Parameter] public EventCallback<DossierTrayResponse> increaseTray { get; set; }
        private DisciplinaryOpennessRegisterDTO disciplinaryOpenness = new DisciplinaryOpennessRegisterDTO();

        private bool pageLoading = false;
        String typeDocument = "AperturamientoDisciplinario";
        private List<MediaRegisterDTO> annexes = new();
        private string dossierId = default!;

        List<ProsecutorUserInfoResponse> prosecutors { get; set; } = new List<ProsecutorUserInfoResponse>();

        protected override async Task OnInitializedAsync()
        {
            disciplinaryOpenness.Participants = new List<Participant>() { new Participant() { Index = 0, Name = "" } };
            disciplinaryOpenness.ChargedDeeds = new List<Deed>() { new Deed() { Index = 0, Description = "" } };

            await GetUserRequestInformationAsync();

            pageLoading = false;

        }

        private void addParticipant()
        {
            disciplinaryOpenness.Participants.Add(new Participant() { Index = (disciplinaryOpenness.Participants.Count) });
            StateHasChanged();
        }

        private void removeParticipant(int index)
        {
            disciplinaryOpenness.Participants.RemoveAt(index);
            StateHasChanged();
        }
        private void addDeed()
        {
            disciplinaryOpenness.ChargedDeeds.Add(new Deed() { Index = (disciplinaryOpenness.ChargedDeeds.Count) });
            StateHasChanged();
        }
        private void removeDeed(int index)
        {
            disciplinaryOpenness.ChargedDeeds.RemoveAt(index);
            StateHasChanged();
        }

        //Consulta de notarios
        private async Task<IEnumerable<Solicitor>> Match(string searchtext)
        {
            var httpResponse = await httpRepository.GetAsync<List<Solicitor>>($"api/solicitors/filter?term={searchtext}");
            if (httpResponse.Error)
            {
                return new List<Solicitor>();
            }
            else
            {
                return httpResponse.Response!;
            }
        }

        public void HandleInvalidSubmit()
        {
            swalFireRepository.ShowErrorSwalFireAsync("Por favor, verifique los Datos Ingresados");
        }

        private DossierWrapper GetDocumentRegister()
        {
            var complaintRequestContent = Mapper.Map<ComplaintRequestResponseContent>(disciplinaryOpenness);
            var complaint = new ComplaintRequestResponse(complaintRequestContent, annexes);

            var documentRegister = new DossierWrapper(dossierId, complaint);

            return documentRegister;
        }

        private async Task<DisciplinaryOpenness?> ShowLoadingDialogAsync(DossierWrapper documentRegister)
        {
            string dialogTitle = $"Realizando el registro de su denuncia, por favor espere...";

            var toRegister = () => RegisterDisciplinaryOpennessRequestAsync(documentRegister);

            return await dialogContentRepository.ShowLoadingDialogAsync(toRegister, dialogTitle);

        }

        private async Task RegisterDisciplinaryOpennessRequestAsync()
        {
            var documentRegister = GetDocumentRegister();

            var registeredComplaint = await ShowLoadingDialogAsync(documentRegister);

            if (registeredComplaint is null) return;

            await swalFireRepository.ShowSuccessfulSwalFireAsync($"Se pudo registrar el aperturamiento disciplinario de manera satisfactoria");
        }

        private async Task<DisciplinaryOpenness?> RegisterDisciplinaryOpennessRequestAsync(DossierWrapper documentRegister)
        {
            try
            {
                var disciplinaryResponse = await httpRepository.PostAsync<DossierWrapper, DisciplinaryOpenness>("api/documents/disciplinary-openness", documentRegister);

                if (disciplinaryResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar el aperturamiento disciplinario");
                }

                return disciplinaryResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo registar el aperturamiento disciplinario");
                return null;
            }
        }

        private async Task<AutocompletedSolicitorResponse> GetSolicitorAsync(string solicitorId)
        {
            try
            {
                var solicitorResponse = await httpRepository.GetAsync<AutocompletedSolicitorResponse>($"api/solicitors/{solicitorId}");

                if (solicitorResponse.Error)
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                }

                return solicitorResponse.Response!;
            }
            catch (Exception)
            {

                await swalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los tipos de solicitudes del sistema");
                return new();
            }
        }

        private async Task GetUserRequestInformationAsync()
        {
            var userTray = WorkPlaceItems.First(workItem => workItem.OriginPlace != "tools");

            disciplinaryOpenness.Client = userTray.Client;

            var dossierTray = userTray.Value as DossierTrayResponse;

            //var documentContent = JsonSerializer.Deserialize<InitialRequestContentDTO>(JsonSerializer.Serialize(dossierTray!.Document!.Content));

            //disciplinaryOpenness.Solicitor = await GetSolicitorAsync(documentContent!.SolicitorId);
            dossierId = dossierTray!.DossierId;
        }

        private void GetSolicitorResponse(AutocompletedSolicitorResponse AutocompletedSolicitorResponse)
        {
            disciplinaryOpenness.Solicitor = AutocompletedSolicitorResponse;
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
                swalFireRepository.ShowErrorSwalFireAsync("No se puede registrar la solicitud de Denuncia, por favor verifique los datos ingresados").GetAwaiter();

                return true;
            }

            RegisterDisciplinaryOpennessRequestAsync().GetAwaiter();

            return false;
        }
    }
}