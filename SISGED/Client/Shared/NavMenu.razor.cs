using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SISGED.Shared.Entities;

namespace SISGED.Client.Shared
{
    public partial class NavMenu
    {
        [Parameter]
        public List<Permission> InterfacePermissions { get; set; } = default!;

        private static NavLinkMatch GetLinkMatch(Permission permission)
        {
            return permission.Url == string.Empty ? NavLinkMatch.All : NavLinkMatch.Prefix;
        }
        
    }
    
}