namespace SISGED.Server.Services.Contracts
{
    public interface ITrayService : IGenericService
    {
        Task RegisterUserTrayAsync(string type, string userId);
    }
}
