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
            services.AddSingleton<IMediaService, MediaService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITrayService, TrayService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISolicitorService, SolicitorService>();
            services.AddScoped<IDossierService, DossierService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IAssistantService, AssistantService>();
            services.AddScoped<IPublicDeedsService, PublicDeedsService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IStepService, StepService>();
            services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            services.AddScoped<ISolicitorDossierService, SolicitorDossierService>();
            services.AddScoped<IDocumentVersionService, DocumentVersionService>();
            services.AddScoped<IDocumentProcessService, DocumentProcessService>();
            services.AddScoped<IDocumentEvaluationService, DocumentEvaluationService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
