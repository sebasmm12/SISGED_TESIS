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
using AutoMapper;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;
using SISGED.Shared.Validators;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Models.Responses.UserDocument;
using SISGED.Shared.Models.Responses.UserDossier;
using SISGED.Client.Components.Documents.Histories.DossierInfoDialog;

namespace SISGED.Client.Pages.Dossiers
{
    public partial class DossiersList
    {
        [Inject]
        public IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        public IDialogService DialogService { get; set; } = default!;
        [Inject]
        public IFilterRepository<UserDossierFilterDTO> UserDossierRepository { get; set; } = default!;
        [Inject]
        public IDossierRepository DossierRepository { get; set; } = default!;
        [Inject]
        public IDossierStateRepository DossierStateRepository { get; set; } = default!;
        [Inject]
        public UserDossierValidator UserDossierValidator { get; set; } = default!;
        [Inject]
        public IMapper Mapper { get; set; } = default!;

        [Parameter]
        public int PageSize { get; set; } = 5;

        private bool documentsLoading = true;
        private PaginatedUserDossierDTO paginatedUserDossiers = default!;
        private UserDossierFilterDTO userDossierFilter = new();
        private int currentPage = 0;
        private IEnumerable<SelectOption> types = default!;
        private IEnumerable<SelectOption> dossierStates = default!;
        private bool dossierSearchLoading = false;
        private MudForm? documentSearcherForm = default!;

        private int TotalDossiers => (paginatedUserDossiers.Total + PageSize - 1) / PageSize;

        protected override async Task OnInitializedAsync()
        {
            paginatedUserDossiers = await GetDossiersAsync();

            types = DossierRepository.GetDossierTypes();
            dossierStates = DossierStateRepository.GetDossierStates();

            documentsLoading = false;
        }

        private async Task ChangePage(int page)
        {
            currentPage = page - 1;

            paginatedUserDossiers = await GetDossiersAsync();
        }

        private async Task SearchDocumentsAsync()
        {
            dossierSearchLoading = true;

            await documentSearcherForm!.Validate();

            if (!documentSearcherForm.IsValid)
            {
                dossierSearchLoading = false;

                return;
            }

            paginatedUserDossiers = await GetDossiersAsync();

            dossierSearchLoading = false;
        }

        private async Task<PaginatedUserDossierDTO> GetDossiersAsync()
        {
            try
            {
                var userDocumentQueries = GetQueriesForUserDossiers();

                var solicitorDossiersResponse = await HttpRepository.GetAsync<PaginatedUserDossierResponse>($"api/dossiers/list{userDocumentQueries}");

                if (solicitorDossiersResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los expedientes registrados");
                }

                var response = solicitorDossiersResponse.Response!;
                var documents = Mapper.Map<PaginatedUserDossierDTO>(response);


                return documents;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los documentos registrados");
                return new(new List<UserDossierDTO>(), 0);
            }
        }

        private string GetQueriesForUserDossiers()
        {

            string userRequestQueries = "?";
            userRequestQueries += $"page={System.Web.HttpUtility.UrlEncode(currentPage.ToString())}";
            userRequestQueries += $"&pagesize={System.Web.HttpUtility.UrlEncode(PageSize.ToString())}";

            var userDocumentFilters = UserDossierRepository.ConvertToFilters(userDossierFilter);

            if (userDocumentFilters.Count == 0) return userRequestQueries;

            userRequestQueries += "&" + userDocumentFilters.Select(filter => $"{filter.Key}={System.Web.HttpUtility.UrlEncode(filter.Value.ToString())}")
                .Aggregate((current, keys) => $"{current}&{keys}");

            return userRequestQueries;
        }

        async Task DossierDerivationHistoryInfo(UserDossierDTO dossier)
        {
            var parameters = new DialogParameters { ["Derivations"] = dossier.Derivations };

            var options = new DialogOptions() { FullScreen = true, CloseButton = true };

            var dialog = DialogService.Show<DossierInfoDerivationsDialog>("Derivaciones", parameters, options);
            _ = await dialog.Result;
        }

        async Task DossierDocumentHistoryInfo(UserDossierDTO dossier)
        {
            var parameters = new DialogParameters { ["Documents"] = dossier.Documents };

            var options = new DialogOptions() { FullScreen = true, CloseButton = true };

            var dialog = DialogService.Show<DossierInfoDocumentsDialog>("Línea de Tiempo de Documentos", parameters, options);
            _ = await dialog.Result;
        }
    }
}