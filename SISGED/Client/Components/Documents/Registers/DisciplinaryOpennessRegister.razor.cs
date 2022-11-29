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
using SISGED.Shared.Models.Responses.Document;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Shared.Models.Responses.Account;
using Newtonsoft.Json;
using SISGED.Shared.Validators;
using SISGED.Shared.DTOs;

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

        private MudForm? requestForm = default!;

        //Variables de sesion
        [CascadingParameter] WorkEnvironment workspace { get; set; }
        [CascadingParameter(Name = "SesionUsuario")] protected SessionAccountResponse session { get; set; }
        //Datos del formulario
        [Parameter] public EventCallback<DossierTrayResponse> increaseTray { get; set; }
        private DisciplinaryOpennessResponse disciplinaryOpenness = new DisciplinaryOpennessResponse();

        private bool loadProcess = false;
        private bool pageLoading = false;
        int step = 0;
        int substep = 0;
        String typeDocument = "AperturamientoDisciplinario";
        private List<MediaRegisterDTO> files = new();
        private List<MediaRegisterDTO> annexes = new();

        List<ProsecutorUserInfoResponse> prosecutors { get; set; } = new List<ProsecutorUserInfoResponse>();

        protected override async Task OnInitializedAsync()
        {
            disciplinaryOpenness.Content.Participants = new List<Participant>() { new Participant() { Index = 0, Name = "" } };
            disciplinaryOpenness.Content.ChargedDeeds = new List<Deed>() { new Deed() { Index = 0, Description = "" } };
            disciplinaryOpenness.Content.URLAnnex = new List<string>();

            foreach (string u in disciplinaryOpenness.Content.URLAnnex)
            {
                if (!string.IsNullOrWhiteSpace(u))
                {
                    disciplinaryOpenness.Content.URLAnnex = null;
                }
            }
            if (!string.IsNullOrEmpty(disciplinaryOpenness.Content.URL))
            {
                disciplinaryOpenness.Content.URL = null;
            }
            var httpResponse = await httpRepository.GetAsync<List<ProsecutorUserInfoResponse>>("api/users/prosecutors");
            if (!httpResponse.Error)
            {
                prosecutors = httpResponse.Response!;
            }
            else
            {
                Console.WriteLine("Ocurrio un error");
            }
            //substep = 1;
            //String message = "";

            //if (typeDocument != workspace.asistente.tipodocumento)
            //{
            //    message = "El documento que ha elegido no es el indicado para el expediente";
            //}
            //else
            //{
            //    message = workspace.asistente.pasos.documentos.Find(x => x.tipo == workspace.asistente.tipodocumento).pasos.ElementAt(step).subpaso.ElementAt(substep).descripcion;

            //    await workspace.UpdatePasoAndSubPasoNormal(step, substep, workspace.asistente.tipodocumento);
            //}

            //Task voiceMessage = workspace.VoiceMessage(message);

            //workspace.ChangeMessage(message);

            //await voiceMessage;

            pageLoading = false;

        }

        private void addParticipant()
        {
            disciplinaryOpenness.Content.Participants.Add(new Participant() { Index = (disciplinaryOpenness.Content.Participants.Count) });
            StateHasChanged();
        }

        private void removeParticipant(int index)
        {
            disciplinaryOpenness.Content.Participants.RemoveAt(index);
            StateHasChanged();
        }
        private void addDeed()
        {
            disciplinaryOpenness.Content.ChargedDeeds.Add(new Deed() { Index = (disciplinaryOpenness.Content.ChargedDeeds.Count) });
            StateHasChanged();
        }
        private void removeDeed(int index)
        {
            disciplinaryOpenness.Content.ChargedDeeds.RemoveAt(index);
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

        public async Task HandleValidSubmit()
        {
            loadProcess = true;
            if (disciplinaryOpenness.Content.URL != "" && disciplinaryOpenness.Content.URL != null)
            {
                DossierWrapper dossierWrapper = new DossierWrapper();
                dossierWrapper.Document = disciplinaryOpenness;
                dossierWrapper.Id = workspace.SelectedTray.DossierId;
                dossierWrapper.CurrentUserId = session.User.Id;
                dossierWrapper.InputDocument = workspace.SelectedTray.Document!.Id;

                var httpResponse = await httpRepository.PostAsync<DossierWrapper, DisciplinaryOpenness>($"api/documents/documentoad", dossierWrapper);
                if (!httpResponse.Error)
                {
                    DossierTrayResponse dossierTray = new DossierTrayResponse();
                    dossierTray = workspace.SelectedTray;
                    DisciplinaryOpenness updateDisciplinaryOpenness = new DisciplinaryOpenness();
                    updateDisciplinaryOpenness = httpResponse.Response!;
                    dossierTray.Document.Id = updateDisciplinaryOpenness!.Id;
                    dossierTray.Document.Type = updateDisciplinaryOpenness.Type;
                    dossierTray.Document.ContentsHistory = updateDisciplinaryOpenness.ContentsHistory;
                    dossierTray.Document.ProcessesHistory = updateDisciplinaryOpenness.ProcessesHistory;
                    dossierTray.Document.Content = updateDisciplinaryOpenness.Content;
                    dossierTray.Document.State = updateDisciplinaryOpenness.State;

                    SISGED.Shared.Models.Item outItem = new SISGED.Shared.Models.Item()
                    {
                        Name = dossierTray.Type!,
                        Value = dossierTray,
                        Icon = "alarm_add",
                        Description = ((DocumentResponse)dossierTray.Document).Type,
                        CurrentPlace = "workspace",
                        OriginPlace = "inputs",
                        Client = dossierTray.Client!,
                        ItemStatus = "registrado"
                    };
                    workspace.UpdateRegisteredDocument(outItem);
                    workspace.UpdateTools("Registrar Documento");
                    DocumentResponse documentRequest = new DocumentResponse();
                    documentRequest.Id = updateDisciplinaryOpenness.Id;
                    documentRequest.Type = updateDisciplinaryOpenness.Type;
                    documentRequest.State = updateDisciplinaryOpenness.State;
                    documentRequest.Content = JsonConvert.SerializeObject(updateDisciplinaryOpenness.Content);
                    documentRequest.ContentsHistory = updateDisciplinaryOpenness.ContentsHistory;
                    documentRequest.ProcessesHistory = updateDisciplinaryOpenness.ProcessesHistory;
                    documentRequest.UrlAnnex = updateDisciplinaryOpenness.AttachedUrls;

                    var docdto = JsonConvert.SerializeObject(documentRequest);
                    //Console.WriteLine(docdto);
                    DocumentRequest doc = new DocumentRequest();
                    doc = JsonConvert.DeserializeObject<DocumentRequest>(docdto)!;

                    workspace.SelectedTray.DocumentObjects!.Add(documentRequest);
                    workspace.SelectedTray.Document = documentRequest;
                    StateHasChanged();

                    await swalFireRepository.ShowSuccessfulSwalFireAsync("Aperturamiento Disciplinario registrado correctamente")!;
                    substep = 2;

                    Int32 pasoAntiguo = step;
                    step = 1;
                    substep = 0;
                    //Enviar paso=0+0,subpaso=2,idexpediente

                    /*String tipodocumentoantiguo = workspace.asistente.tipodocumento;

                    await workspace.UpdatePasoAndSubPasoFinnally(step, substep, workspace.asistente.tipodocumento, pasoAntiguo, tipodocumentoantiguo);

                    String messageFinal = workspace.asistente.pasos.documentos
                            .Find(x => x.tipo == workspace.asistente.tipodocumento).pasos.ElementAt(step).descripcion;

                    Task voiceMessage = workspace.VoiceMessage(messageFinal);

                    workspace.ChangeMessage(messageFinal);

                    await voiceMessage;*/
                    //Enviar paso=0,subpaso=2, idexp
                }
                else
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("Error en el servidor. Intentelo de nuevo");
                }
            }
            else
            {
                await swalFireRepository.ShowErrorSwalFireAsync("Debe subir un PDF obligatoriamente");
            }
            loadProcess = false;
        }

        public void HandleInvalidSubmit()
        {
            loadProcess = false;
            swalFireRepository.ShowErrorSwalFireAsync("Por favor, verifique los Datos Ingresados");
        }

        private async Task RegisterRequestAsync()
        {
            await requestForm!.Validate();

            if (!requestForm.IsValid)
            {
                HandleInvalidSubmit();
                return;
            }

            await HandleValidSubmit();
        }
    }
}