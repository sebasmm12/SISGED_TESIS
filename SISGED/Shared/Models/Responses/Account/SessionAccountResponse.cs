using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Account
{
    public class SessionAccountResponse
    {
        public List<Permission> ToolPermissions { get; set; } = default!;
        public List<Permission> InterfacePermissions { get; set; } = default!;
        public List<Item> UsableTools { get; set; } = default!;
        public List<Item> Inputs { get; set; } = default!;
        public List<Item> Outputs { get; set; } = default!;
        public string Role { get; set; } = default!;
        public Entities.User User { get; set; } = default!;
    }
}
