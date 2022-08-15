using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using SISGED.Server.Helpers.Infrastructure;
using SISGED.Server.Helpers.Middlewares;
using SISGED.Server.Services.Contracts;
using SISGED.Server.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// SeriLog Configuration
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

try
{
    builder.Host.UseSerilog((hostBuilder, serviceProvider) =>
    {
        serviceProvider.ReadFrom.Configuration(hostBuilder.Configuration);
    });

    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // Infrastructure Dependency Injection
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

    // Mongo Dependency Injection
    builder.Services.AddSingleton<IMongoClient>(mongoClient =>
                                                new MongoClient(builder.Configuration.GetValue<string>("SisgedDBSettings:ConnectionString")));


    builder.Services.Configure<SisgedDBSettings>(builder.Configuration.GetSection(nameof(SisgedDBSettings)));


    builder.Services.AddSingleton<ISisgedDBSettings>(sisgedServiceProvider => sisgedServiceProvider.GetRequiredService<IOptions<SisgedDBSettings>>().Value);


    builder.Services.AddScoped(serviceProvider =>
    {
        var client = serviceProvider.GetRequiredService<IMongoClient>();
        var sisgedSettings = serviceProvider.GetRequiredService<ISisgedDBSettings>();
        return client.GetDatabase(sisgedSettings.DatabaseName);
    });

    // Service Dependency Injection
    builder.Services.AddServices();

    // Swagger Dependency Injection
    builder.Services.AddSwaggerGen(swaggerGen =>
    {
        swaggerGen.CustomSchemaIds(type => type.ToString());

        swaggerGen.SwaggerDoc("v1", new()
        {
            Title = "SisgedAPI",
            Version = "v1",
            Description = "Description of the SISGED API with all endpoints",
            License = new() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") },
        });

        swaggerGen.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header
        });

        swaggerGen.AddSecurityRequirement(new()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
    


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    // Middleware for Swagger
    app.UseSwagger();
    app.UseSwaggerUI(swaggerUI =>
    {
        swaggerUI.SwaggerEndpoint("/swagger/v1/swagger.json", "SISGED API v1");
    });

    // Middleware for Serilog
    app.UseSerilogRequestLogging();

    // Own Middlewares
    app.UseHttpLogger();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "The SISGED Web API wasn't executed");
}
finally
{
    Log.Information("Shut down was completed successfully");
    Log.CloseAndFlush();
}