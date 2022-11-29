using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Components.Documents
{
    public partial class DocumentAnnex
    {
        [Inject]
        private ISwalFireRepository SwalFireRepository { get; set; } = default!;

        [Inject]
        private IAnnexFactory AnnexFactory { get; set; } = default!;

        [Parameter]
        public List<MediaRegisterDTO> Annexes { get; set; } = new();

        private static readonly string defaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
        private string dragClass = defaultDragClass;

        private AnnexPreview GetAnnexPreview(string annexExtension)
        {
            var annexPreview = AnnexFactory.GetAnnexPreview(annexExtension);

            return annexPreview;
        }

        private void DeleteAnnex(string name)
        {
            int annexIndex = Annexes.FindIndex(annex => annex.Name == name);

            Annexes.RemoveAt(annexIndex);
        }

        private async Task OnInputFileChanged(InputFileChangeEventArgs inputFile)
        {
            ClearDragClass();

            var files = inputFile.GetMultipleFiles();

            if(Annexes.Count + files.Count > 5)
            {
                await SwalFireRepository.ShowWarningSwalFireAsync("Solamente se puede ingresar hasta 5 archivos");
                return;
            }

            var filteredFiles = files.Where(file => !Annexes.Any(annex => annex.Name == file.Name));

            var fileTasks = filteredFiles.Select(file => GetAnnexAsync(file, file.Size));

            var annexes = await Task.WhenAll(fileTasks);

            Annexes.AddRange(annexes);

        }

        private static async Task<MediaRegisterDTO> GetAnnexAsync(IBrowserFile file, long maxFileSize)
        {
            var fileBytes = new byte[file.Size];

            await file.OpenReadStream(maxFileSize).ReadAsync(fileBytes);

            string fileContent = Convert.ToBase64String(fileBytes);
            string filePath = Path.GetExtension(file.Name);

            return new(fileContent, filePath, file.Name);
        }

        private void SetDragClass()
        {
            dragClass = $"{ defaultDragClass } mud-border-primary";
        }

        private void ClearDragClass()
        {
            dragClass = defaultDragClass;
        }
    }
}