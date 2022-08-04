namespace SISGED.Server.Services.Contracts
{
    public interface ISisgedDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        
    }
}
