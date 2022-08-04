using SISGED.Server.Services.Contracts;

namespace SISGED.Server.Services.Repositories
{
    public class SisgedDBSettings : ISisgedDBSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
    }
}
