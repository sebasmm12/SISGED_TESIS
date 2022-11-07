using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SISGED.Client;
using SISGED.Client.Services.Contracts;
using SISGED.Client.Services.Repositories;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

ConfigureServices(builder.Services);

builder.Services.AddMudServices();

await builder.Build().RunAsync();


static void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<ISwalFireRepository, SwalFireRepository>();
    services.AddScoped<IHttpRepository, HttpRepository>();

    services.AddTransient<ToolWindowStrategy>();
}

    