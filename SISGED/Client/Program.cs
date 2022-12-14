using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SISGED.Client;
using SISGED.Client.Helpers;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;
using SISGED.Shared.Validators;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

ConfigureServices(builder.Services);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddMudServices();

await builder.Build().RunAsync();


static void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<ISwalFireRepository, SwalFireRepository>();
    services.AddScoped<IHttpRepository, HttpRepository>();
    services.AddScoped<IDocumentRepository, DocumentRepository>();
    services.AddScoped<ISolicitorRepository, SolicitorRepository>();
    services.AddScoped<IDialogContentRepository, DialogContentRepository>();
    services.AddScoped<IAnnexFactory, AnnexFactory>();

    services.AddTransient<ToolWindowStrategy>();
    services.AddTransient<DocumentStrategy>();

    services.AddTransient<UserRequestRegisterValidator>();
    services.AddTransient<ComplaintRequestValidator>();
    services.AddTransient<ResolutionRegisterValidator>();
    services.AddTransient<DisciplinaryOpennessRegisterValidator>();
    services.AddTransient<SolicitorDossierRequestRegisterValidator>();
}

    