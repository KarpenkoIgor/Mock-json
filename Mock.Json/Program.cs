using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

/*builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    var kestrelSection = context.Configuration.GetSection("Kestrel");

    serverOptions.Configure(kestrelSection)
        .Endpoint("HttpsInlineCertFile", endpointOptions =>
        {
            var certificateSection = kestrelSection.GetSection("Endpoints:HttpsInlineCertFile:Certificate");
            var certificatePath = certificateSection["Path"];
            var certificatePassword = certificateSection["Password"];

            var certificate = new X509Certificate2(certificatePath, certificatePassword);

            endpointOptions.HttpsOptions.ServerCertificate = certificate;
        });
});
*/


try
{
    Log.Information("Starting Mock.Json service");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("serilog.json");
            config.AddJsonFile($"appsettings.{Environment.MachineName}.json", true);
        })
        .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var app = builder.Build();

    app.UseSerilogRequestLogging();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}