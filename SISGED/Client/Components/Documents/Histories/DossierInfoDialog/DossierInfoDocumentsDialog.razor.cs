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
using SISGED.Client.Components.Trays;
using SISGED.Client.Components.Documents.Histories;
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
using SISGED.Shared.Entities;
using SISGED.Client.Services.Repositories;
using SISGED.Client.Services.Contracts;

namespace SISGED.Client.Components.Documents.Histories.DossierInfoDialog
{
    public partial class DossierInfoDocumentsDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public List<UserDocumentDTO> Documents { get; set; } = new List<UserDocumentDTO>();
        [Inject]
        public IDialogContentRepository DialogContentRepository { get; set; } = default!;
        [Inject]
        public IDocumentRepository DocumentRepository { get; set; } = default!;
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task ShowDocumentVersionHistoryAsync(UserDocumentDTO document)
        {
            var dialogParameters = new List<DialogParameter>() { new("DocumentId", document.Id), new("PageSize", 5) };

            await DialogContentRepository.ShowDialogAsync<DocumentsVersion>(dialogParameters, "Historial de versiones");
        }


        private async Task ShowDocumentProcessHistoryAsync(UserDocumentDTO document)
        {
            var dialogParameters = new List<DialogParameter>() { new("DocumentId", document.Id), new("PageSize", 5) };

            await DialogContentRepository.ShowDialogAsync<DocumentsProcess>(dialogParameters, "Historial de procesos");
        }

        private async Task ShowDocumentInfoAsync(UserDocumentDTO document)
        {
            var dialogParameters = new List<DialogParameter>() { new("DocumentId", document.Id) };

            Type documentInfoType = DocumentRepository.GetDocumentInfoType(document.Type);

            await DialogContentRepository.ShowDialogAsync(documentInfoType, dialogParameters, "Información del Documento");
        }
    }
}