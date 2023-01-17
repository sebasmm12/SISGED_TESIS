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
using SISGED.Client.Services.Contracts;
using SISGED.Shared.Models.Responses.Account;

namespace SISGED.Client.Shared
{
    public partial class UserMenu
    {
        [Inject]
        private ILoginRepository LoginRepository { get; set; } = default!;
        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;
        [CascadingParameter]
        public SessionAccountResponse SessionAccount { get; set; } = default!;

        private async Task LogOutAsync()
        {
            await LoginRepository.Logout();
            navigationManager.NavigateTo("/Login", true);

        }
    }
}