using SISGED.Shared.Entities;

namespace SISGED.Shared.Models.Responses.Account
{
    public class SessionAccountResponse
    {
        public SessionAccountResponse()
        {
            
        }

        public SessionAccountResponse(List<Permission> toolPermissions, List<Permission> interfacePermissions, string role, Entities.User user)
        {
            ToolPermissions = toolPermissions;
            InterfacePermissions = interfacePermissions;
            Role = role;
            User = user;
        }

        public List<Permission> ToolPermissions { get; set; } = default!;
        public List<Permission> InterfacePermissions { get; set; } = default!;
        //public List<Item> UsableTools { get; set; } = default!;
        //public List<Item> Inputs { get; set; } = default!;
        //public List<Item> Outputs { get; set; } = default!;
        public string Role { get; set; } = default!;
        public Entities.User User { get; set; } = default!;

        public Entities.User GetUser()
        {
            return User;
        }
        
        public UserData GetClient()
        {
            return User.Data;
        }

        public string GetDocumentNumber()
        {
            return User.Data.DocumentNumber;
        }

        public string GetDocumentType()
        {
            return User.Data.DocumentType;
        }
    }
}
