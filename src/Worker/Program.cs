// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Nikiforoval.CA.Template.Application;
using Nikiforoval.CA.Template.Application.SharedKernel.Interfaces;
using Nikiforoval.CA.Template.Infrastructure;
using Nikiforoval.CA.Template.Worker;
using Serilog;
using Serilog.Core;

var host = CreateHostBuilder(args).Build();
var hostEnvironment = host.Services.GetRequiredService<IHostEnvironment>();
var applicationName = hostEnvironment.ApplicationName;
var environmentName = hostEnvironment.EnvironmentName;
Log.Logger = CreateLogger(host);

try
{
    LogLifecycle("Started {Application} in {Environment} mode.");
    await host.RunAsync();
    LogLifecycle("Stopped {Application} in {Environment} mode.");
    return 0;
}
catch (Exception exception)
{
    Log.Fatal(
        exception,
        "{Application} terminated unexpectedly in {Environment} mode.",
        applicationName,
        environmentName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseDefaultServiceProvider((context, options) =>
    {
        var isDevelopment = context.HostingEnvironment.IsDevelopment();
        options.ValidateScopes = isDevelopment;
        options.ValidateOnBuild = isDevelopment;
    })
    .ConfigureServices((hostContext, services)
        => ConfigureServices(services, hostContext.Configuration));

static IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddApplication();
    services.AddInfrastructure(configuration);
    services.AddSingleton<ICurrentUserService, WorkerUserService>();
    //services.AddMassTransit(configuration);

    return services;
}

void LogLifecycle(string msg) => Log.Information(msg, applicationName, environmentName);

Logger CreateLogger(IHost host)
{
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    return new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application",
            configuration["DOTNET_APPLICATIONNAME"] ?? hostEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", environmentName)
        .CreateLogger();
}
