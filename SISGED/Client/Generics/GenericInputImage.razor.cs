using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Generics
{
    public partial class GenericInputImage
    {
        [Parameter]
        public string Label { get; set; } = "Imagen";

        [Parameter]
        public string? ImageUrl { get; set;}
        
        [Parameter]
        public EventCallback<MediaRegisterDTO> SelectedImage { get; set; }

        [Parameter]
        public string Icon { get; set; } = "fas fa-signature";

        private string imagePreLoad = default!;

        private async Task UploadFiles(InputFileChangeEventArgs inputFileChangeEvent)
        {
            var files = inputFileChangeEvent.GetMultipleFiles();

            var fileTasks = files.Select(file => GetAnnexAsync(file, file.Size));

            var images = await Task.WhenAll(fileTasks);

            var image = images.First();

            ImageUrl = null;
            imagePreLoad = image.Content;

            await SelectedImage.InvokeAsync(image);
        }

        private static async Task<MediaRegisterDTO> GetAnnexAsync(IBrowserFile file, long maxFileSize)
        {
            var fileBytes = new byte[file.Size];

            await file.OpenReadStream(maxFileSize).ReadAsync(fileBytes);

            string fileContent = Convert.ToBase64String(fileBytes);
            string filePath = Path.GetExtension(file.Name);

            return new(fileContent, filePath, file.Name);
        }
    }
}