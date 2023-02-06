using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SISGED.Client;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.DTOs;
using SISGED.Shared.Validators;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

ConfigureServices(builder.Services);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();


static void ConfigureServices(IServiceCollection services)
{
    services.AddAuthorizationCore();

    services.AddScoped<ISwalFireRepository, SwalFireRepository>();
    services.AddScoped<IHttpRepository, HttpRepository>();
    services.AddScoped<IDocumentRepository, DocumentRepository>();
    services.AddScoped<IDossierRepository, DossierRepository>();
    services.AddScoped<IDocumentStateRepository, DocumentStateRepository>();
    services.AddScoped<IFilterRepository<SolicitorFilter>, SolicitorRepository>();
    services.AddScoped<IFilterRepository<UserDocumentFilterDTO>, UserDocumentRepository>();
    services.AddScoped<IDialogContentRepository, DialogContentRepository>();
    services.AddScoped<IAnnexFactory, AnnexFactory>();
    services.AddScoped<IBadgeFactory, BadgeFactory>();
    services.AddScoped<ILocalStorageRepository, LocalStorageRepository>();

    services.AddScoped<LoginRepository>();
    services.AddScoped<AuthenticationStateProvider, LoginRepository>();
    services.AddScoped<ILoginRepository, LoginRepository>();

    services.AddScoped<ITokenRenewer, TokenRenewer>();

    services.AddTransient<ToolWindowStrategy>();
    services.AddTransient<DocumentStrategy>();
    services.AddTransient<DocumentGeneratorStrategy>();

    services.AddTransient<UserRequestRegisterValidator>();
    services.AddTransient<ComplaintRequestValidator>();
    services.AddTransient<DictumValidator>();
    services.AddTransient<SolicitorDossierShipmentValidator>();
    services.AddTransient<ResolutionRegisterValidator>();
    services.AddTransient<DisciplinaryOpennessRegisterValidator>();
    services.AddTransient<SolicitorDossierRequestRegisterValidator>();
    services.AddTransient<UserLoginValidator>();
    services.AddTransient<DocumentDerivationValidator>();
    services.AddTransient<DocumentEvaluationValidator>();
    services.AddTransient<UserSelfRegisterValidator>();
    services.AddTransient<UserDocumentValidator>();
}

    