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
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Models.Responses.Dossier;
using SISGED.Shared.Models.Requests.Documents;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Responses.Document;
using Newtonsoft.Json;
using SISGED.Shared.Validators;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class SolicitorDossierRequestRegister
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;
        [Inject]
        public SolicitorDossierRequestRegisterValidator SolicitorDossierRequestRegisterValidator { get; set; } = default!;

        //Variables de sesion
        [CascadingParameter] WorkEnvironment workspace { get; set; }
        [CascadingParameter(Name = "SesionUsuario")] protected SessionAccountResponse session { get; set; }

        private MudForm? requestForm = default!;

        //Datos del formulario
        [Parameter] public EventCallback<DossierTrayResponse> increaseTray { get; set; }
        private SolicitorDossierRequestResponse solicitorDossierRequest = new SolicitorDossierRequestResponse();
        List<string> names = new List<string>();
        private List<MediaRegisterDTO> files = new();
        private bool loadprocess = false;
        private bool pageLoading = true;
        int step = 0;
        int substep = 0;
        String typeDocument = "SolicitudExpedienteNotario";

        protected override async Task OnInitializedAsync()
        {
            solicitorDossierRequest.Content.URLAnnex = new List<string>();
            foreach (string u in solicitorDossierRequest.Content.URLAnnex)
            {
                if (!string.IsNullOrWhiteSpace(u))
                {
                    solicitorDossierRequest.Content.URLAnnex = null;
                }
            }
            substep = 1;

            //Enviar paso=0, subpaso=1, idex
            String message = "";

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

        public async Task HandleValidSubmit()
        {
            loadprocess = true;
            DossierWrapper dossierWrapper = new DossierWrapper();
            dossierWrapper.Document = solicitorDossierRequest;
            dossierWrapper.Id = workspace.SelectedTray.DossierId;
            dossierWrapper.CurrentUserId = session.User.Id;
            dossierWrapper.InputDocument = workspace.SelectedTray.Document!.Id;

            var httpResponse = await httpRepository.PostAsync<DossierWrapper, SolicitorDossierRequest>($"api/document/documentoSEN", dossierWrapper);
            if (!httpResponse.Error)
            {
                DossierTrayResponse dossierTray = new DossierTrayResponse();
                dossierTray = workspace.SelectedTray;
                SolicitorDossierRequest updateSolicitorDossierRequest = new SolicitorDossierRequest();
                updateSolicitorDossierRequest = httpResponse.Response;
                dossierTray.Document.Id = updateSolicitorDossierRequest.Id;
                dossierTray.Document.Type = updateSolicitorDossierRequest.Type;
                dossierTray.Document.ContentsHistory = updateSolicitorDossierRequest.ContentsHistory;
                dossierTray.Document.ProcessesHistory = updateSolicitorDossierRequest.ProcessesHistory;
                dossierTray.Document.Content = updateSolicitorDossierRequest.Content;
                dossierTray.Document.State = updateSolicitorDossierRequest.State;
                //expedientebandeja.documentosobj.Add(expedientebandeja.documento);
                SISGED.Shared.Models.Item itemSalida = new SISGED.Shared.Models.Item()
                {
                    Name = dossierTray.Type,
                    Value = dossierTray,
                    Icon = "alarm_add",
                    Description = ((DocumentResponse)dossierTray.Document).Type,
                    CurrentPlace = "workspace",
                    OriginPlace = "inputs",
                    Client = dossierTray.Client,
                    ItemStatus = "registrado"
                };

                workspace.UpdateRegisteredDocument(itemSalida);
                workspace.UpdateTools("Registrar Documento");

                DocumentResponse doc = new DocumentResponse();
                doc.Id = updateSolicitorDossierRequest.Id;
                doc.Type = updateSolicitorDossierRequest.Type;
                doc.State = updateSolicitorDossierRequest.State;
                doc.Content = JsonConvert.SerializeObject(updateSolicitorDossierRequest.Content);
                doc.ContentsHistory = updateSolicitorDossierRequest.ContentsHistory;
                doc.ProcessesHistory = updateSolicitorDossierRequest.ProcessesHistory;
                doc.UrlAnnex = updateSolicitorDossierRequest.AttachedUrls;

                workspace.SelectedTray.DocumentObjects.Add(doc);
                workspace.SelectedTray.Document = doc;

                StateHasChanged();
                //workspace.UpdateComponentBandeja(itemSalida);
                await swalFireRepository.ShowSuccessfulSwalFireAsync("Solicitud de expediente de notario registrado correctamente");
                substep = 2;
                //Enviar paso=0, subpaso=2, idexp
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
            }
            else
            {
                await swalFireRepository.ShowErrorSwalFireAsync("Error en el servidor. Intentelo de nuevo");
            }
            loadprocess = false;
        }

        private async Task<IEnumerable<Solicitor>> Match(string searchtext)
        {
            var httpResponse = await httpRepository.GetAsync<List<Solicitor>>($"api/Solicitor/filter?term={searchtext}");
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
            loadprocess = false;
            swalFireRepository.ShowErrorSwalFireAsync("Por favor, Verifique los Datos Ingresados");
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