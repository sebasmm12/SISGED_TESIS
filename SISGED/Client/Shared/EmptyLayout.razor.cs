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

namespace SISGED.Client.Shared
{
    public partial class EmptyLayout
    {
    }
}