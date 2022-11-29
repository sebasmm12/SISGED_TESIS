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

namespace SISGED.Client.Components.Documents.Registers
{
    public partial class ResolutionRegister
    {
        [Inject]
        private IHttpRepository httpRepository { get; set; } = default!;
        [Inject]
        private ISwalFireRepository swalFireRepository { get; set; } = default!;

        //Variables de sesion
        [CascadingParameter] WorkEnvironment workspace { get; set; }
        [CascadingParameter(Name = "SesionUsuario")] protected SessionAccountResponse session { get; set; }

        //Datos del formulario
        [Parameter] public EventCallback<DossierTrayResponse> increaseTray { get; set; }
        private ResolutionResponse resolution = new ResolutionResponse();
        private EditContext _editContext;
        List<string> names = new List<string>();
        private string tempImage;
        private string tempImage2;
        private bool loadprocess = false;
        int paso = 0;
        int subpaso = 0;
        String typeDocument = "Resolucion";

        protected override async Task OnInitializedAsync()
        {
            resolution.Content.Participants = new List<Participant>() { new Participant() { Index = 0, Name = "" } };
            resolution.Content.UrlAnnex = new List<string>();
            _editContext = new EditContext(resolution);
            foreach (string u in resolution.Content.UrlAnnex)
            {
                if (!string.IsNullOrWhiteSpace(u))
                {
                    tempImage2 = u;
                    resolution.Content.UrlAnnex = null;
                }
            }
            if (!string.IsNullOrEmpty(resolution.Content.Data))
            {
                tempImage = resolution.Content.Data;
                resolution.Content.Data = null;
            }
            subpaso = 1;
            //Enviar paso=0,subpaso=1,idexp
            /*String message = "";

            if (typeDocument != workspace.asistente.tipodocumento)
            {
                message = "El Document que ha elegido no es el indicado para el expediente";
            }
            else
            {
                message = workspace.asistente.pasos.documentos.Find(x => x.tipo == workspace.asistente.tipodocumento).pasos.ElementAt(paso).subpaso.ElementAt(subpaso).descripcion;

                await workspace.UpdatePasoAndSubPasoNormal(paso, subpaso, workspace.asistente.tipodocumento);
            }

            Task voiceMessage = workspace.VoiceMessage(message);

            workspace.ChangeMessage(message);

            await voiceMessage;*/

        }

        private void SelectedImage(string imagenbase64)
        {
            resolution.Content.Data = imagenbase64;
        }

        private void SelectedImage2(string imagenbase64)
        {
            resolution.Content.UrlAnnex.Add(imagenbase64);
        }

        private void FileName(string imagenbase64)
        {
            names.Add(imagenbase64);
        }

        private async Task RemoveFile(string file, int imagen64)
        {
            names.Remove(file);
            resolution.Content.UrlAnnex.RemoveAt(imagen64);
            StateHasChanged();

        }

        private async Task addParticipant()
        {
            Console.WriteLine("INDEX ADD ==> " + resolution.Content.Participants.Count);
            resolution.Content.Participants.Add(new Participant() { Index = (resolution.Content.Participants.Count), Name = "" });
            StateHasChanged();
        }
        private async Task removeParticipant(int index)
        {
            Console.WriteLine("INDEX REMOVE ==> " + index);
            resolution.Content.Participants.RemoveAt(index);
            StateHasChanged();
        }

        void KeyUp(ChangeEventArgs e, string memberName, object val)
        {
            var property = val.GetType().GetProperty(memberName);
            property.SetValue(val, e.Value.ToString());
            var fi = new FieldIdentifier(val, memberName);
            _editContext.NotifyFieldChanged(fi);
        }

        public async Task HandleValidSubmit()
        {
            loadprocess = true;
            if (resolution.Content.Data != "" && resolution.Content.Data != null)
            {
                DossierWrapper dossierWrapper = new DossierWrapper();
                dossierWrapper.Document = resolution;
                dossierWrapper.Id = workspace.SelectedTray.DossierId;
                dossierWrapper.CurrentUserId = session.User.Id;
                dossierWrapper.InputDocument = workspace.SelectedTray.Document.Id;
                var httpResponse = await httpRepository.PostAsync<DossierWrapper, Resolution>($"api/documents/documentor", dossierWrapper);
                if (!httpResponse.Error)
                {
                    DossierTrayResponse dossierTray = new DossierTrayResponse();
                    dossierTray = workspace.SelectedTray;
                    Resolution resolutionDocument = new Resolution();
                    resolutionDocument = httpResponse.Response!;
                    dossierTray.Document.Id = resolutionDocument.Id;
                    dossierTray.Document.Type = resolutionDocument.Type;
                    dossierTray.Document.ContentsHistory = resolutionDocument.ContentsHistory;
                    dossierTray.Document.ProcessesHistory = resolutionDocument.ProcessesHistory;
                    dossierTray.Document.Content = resolutionDocument.Content;
                    dossierTray.Document.State = resolutionDocument.State;
                    //expedientebandeja.documentosobj.Add(expedientebandeja.Document);
                    SISGED.Shared.Models.Item itemSalida = new SISGED.Shared.Models.Item()

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

                    workspace.UpdateRegisteredDocument(itemSalida);
                    workspace.UpdateTools("Registrar Documento");
                    DocumentResponse doc = new DocumentResponse();
                    doc.Id = resolutionDocument.Id;
                    doc.Type = resolutionDocument.Type;
                    doc.State = resolutionDocument.State;
                    doc.Content = JsonConvert.SerializeObject(resolutionDocument.Content);
                    doc.ContentsHistory = resolutionDocument.ContentsHistory;
                    doc.ProcessesHistory = resolutionDocument.ProcessesHistory;
                    doc.UrlAnnex = resolutionDocument.AttachedUrls;
                    workspace.SelectedTray.DocumentObjects!.Add(doc);
                    workspace.SelectedTray.Document = doc;

                    StateHasChanged();
                    await swalFireRepository.ShowSuccessfulSwalFireAsync("Resolución registrada correctamente");
                    subpaso = 2;

                    Int32 pasoAntiguo = paso;
                    paso = 1;
                    subpaso = 0;
                    //Enviar paso=0+0,subpaso=2,idexpediente
                    /*String tipodocumentoantiguo = workspace.asistente.tipodocumento;

                    await workspace.UpdatePasoAndSubPasoFinnally(paso, subpaso, workspace.asistente.tipodocumento, pasoAntiguo, tipodocumentoantiguo);

                    String messageFinal = workspace.asistente.pasos.documentos
                            .Find(x => x.tipo == workspace.asistente.tipodocumento).pasos.ElementAt(paso).descripcion;

                    Task voiceMessage = workspace.VoiceMessage(messageFinal);

                    workspace.ChangeMessage(messageFinal);

                    await voiceMessage;*/
                    //Enviar paso=0,subpaso=2,idexp
                }
                else
                {
                    await swalFireRepository.ShowErrorSwalFireAsync("Error en el servidor, intentelo de nuevo");
                }
            }
            else
            {
                await swalFireRepository.ShowErrorSwalFireAsync("Debe subir un PDF obligatoriamente");
            }
            loadprocess = false;
        }

        public void HandleInvalidSubmit()
        {
            loadprocess = false;
            swalFireRepository.ShowErrorSwalFireAsync("Por favor, Verifique los Datos Ingresados");
        }
    }
}