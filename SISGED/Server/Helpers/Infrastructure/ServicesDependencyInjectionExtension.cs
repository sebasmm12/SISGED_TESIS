using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Repositories;

namespace SISGED.Server.Helpers.Infrastructure
{
    public static class ServicesDependencyInjectionExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IFileStorageService, AzureFileStorageService>();
            services.AddSingleton<IFileService, FileService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITrayService, TrayService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISolicitorService, SolicitorService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IDossierService, DossierService>();
            services.AddScoped<IStatisticService, StatisticService>();

            return services;
        }
    }
}
