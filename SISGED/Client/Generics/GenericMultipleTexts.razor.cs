using Microsoft.AspNetCore.Components;
using MudBlazor;
using SISGED.Client.Helpers;
using SISGED.Shared.DTOs;

namespace SISGED.Client.Generics
{
    public partial class GenericMultipleTexts
    {

        [Parameter]
        public List<TextFieldDTO> TextFields { get; set; } = default!;
        [Parameter]
        public string TextFieldIcon { get; set; } = default!;
        [Parameter]
        public string TextLabel { get; set; } = default!;

        private void AddTextField()
        {
            var lastTextField = TextFields[^1];

            TextFields.Add(new(string.Empty, lastTextField.Index + 1));
        }

        private void DeleteTextField(TextFieldDTO textField)
        {
            TextFields.Remove(textField);
        }

        private static MultipleTextIcon GetTextIcon(TextFieldDTO textField)
        {

            if (textField.Index == 0) return new(Icons.Material.Filled.Add, Color.Success);

            return new(Icons.Material.Filled.Delete, Color.Error);
        }

        private void ExecuteTextFieldAction(TextFieldDTO textFieldDTO)
        {
            if (textFieldDTO.Index == 0)
            {
                AddTextField();
                return;
            }

            DeleteTextField(textFieldDTO);
        }
    }
}