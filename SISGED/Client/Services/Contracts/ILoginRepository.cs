namespace SISGED.Client.Services.Contracts
{
    public interface ILoginRepository
    {
        Task Login(string token);
        Task Logout();
    }
}
