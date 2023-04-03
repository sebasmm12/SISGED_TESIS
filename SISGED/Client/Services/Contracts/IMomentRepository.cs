namespace SISGED.Client.Services.Contracts
{
    public interface IMomentRepository
    {
        Task<string> GetTimeFromNowAsync(DateTime date);
    }
}
