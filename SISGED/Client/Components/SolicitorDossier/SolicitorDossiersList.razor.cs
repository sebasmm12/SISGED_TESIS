using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Entities;
using SISGED.Shared.Models.Queries.SolicitorDossier;
using SISGED.Shared.Models.Responses.SolicitorDossier;

namespace SISGED.Client.Components.SolicitorDossier
{
    public partial class SolicitorDossiersList
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        private IHttpRepository HttpRepository { get; set; } = default!;
        [Inject]
        public ISwalFireRepository SwalFireRepository { get; set; } = default!;
        [Inject]
        private IAnnexFactory AnnexFactory { get; set; } = default!;

        [Parameter]
        public IEnumerable<int> Years { get; set; } = default!;
        [Parameter]
        public List<string> SelectedSolicitorDossierIds { get; set; } = default!;
        [Parameter]
        public int PageSize { get; set; } = 5;
        [Parameter]
        public string SolicitorId { get; set; } = default!;

        private IEnumerable<int> selectedYears = new List<int>();
        private bool solicitorDossierSearchLoading = false;
        private bool solicitorDossiersLoading = true;
        private PaginatedSolicitorDossierResponse paginatedSolicitorDosiers = default!;
        private int currentPage = 0;
        private int TotalSolicitorDossiers => (paginatedSolicitorDosiers.Total + PageSize - 1) / PageSize;


        protected override async Task OnInitializedAsync()
        {
            paginatedSolicitorDosiers = await GetSolicitorDossiersAsync();

            solicitorDossiersLoading = false;
        }

        private bool IsSelected(string solicitorDossierId)
        {
            
            return SelectedSolicitorDossierIds.Contains(solicitorDossierId);
        } 


        private async Task SearchSolicitorDossiersAsync()
        {
            solicitorDossierSearchLoading = true;

            paginatedSolicitorDosiers = await GetSolicitorDossiersAsync();

            SelectedSolicitorDossierIds.Clear();
            solicitorDossierSearchLoading = false;
        }

        private AnnexPreview GetSolicitorDossierPreview(string solicitorDossierUrl)
        {
            var extension = Path.GetExtension(solicitorDossierUrl);

            var solicitorDossierVisualization = AnnexFactory.GetAnnexPreview(extension);

            return solicitorDossierVisualization;
        }

        private void AddOrDeleteSolicitorDossier(SolicitorDossierActionDTO solicitorDossierActionDTO)
        {
            if (solicitorDossierActionDTO.Selected) SelectedSolicitorDossierIds.Add(solicitorDossierActionDTO.SolicitorDossierId);
            else SelectedSolicitorDossierIds.Remove(solicitorDossierActionDTO.SolicitorDossierId);
        } 

        private async Task ChangePage(int page)
        {
            currentPage = page - 1;

            paginatedSolicitorDosiers = await GetSolicitorDossiersAsync();
        }

        private async Task<PaginatedSolicitorDossierResponse> GetSolicitorDossiersAsync()
        {
            try
            {
                string solicitorDossierQueries = GetQueriesForUserRequests();

                var solicitorDossiersResponse = await HttpRepository.GetAsync<PaginatedSolicitorDossierResponse>($"api/solicitorsDossiers{solicitorDossierQueries}");

                if (solicitorDossiersResponse.Error)
                {
                    await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los expedientes del notario");
                }

                return solicitorDossiersResponse.Response!;
            }
            catch (Exception)
            {
                await SwalFireRepository.ShowErrorSwalFireAsync("No se pudo obtener los expedientes del notario");
                return new(new List<SolicitorDossierResponse>(), 0);
            }  
        }

        private string GetQueriesForUserRequests()
        {
            string userRequestQueries = "?";

            userRequestQueries += $"page={System.Web.HttpUtility.UrlEncode(currentPage.ToString())}";
            userRequestQueries += $"&pagesize={System.Web.HttpUtility.UrlEncode(PageSize.ToString())}";
            userRequestQueries += $"&solicitorId={System.Web.HttpUtility.UrlEncode(SolicitorId)}";

            foreach (int selectedYear in selectedYears)
            {
                userRequestQueries += $"&years={System.Web.HttpUtility.UrlEncode(selectedYear.ToString())}";
            }

            return userRequestQueries;
        }
    }
}