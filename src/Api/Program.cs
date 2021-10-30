// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.


using Serilog;
using Serilog.Core;
using Nikiforoval.CA.Template.Api;

var builder = WebApplication.CreateBuilder(args);
var hostEnvironment = builder.Environment;

Log.Logger = CreateLogger(hostEnvironment, builder.Configuration);

builder.Host
    .UseSerilog()
    .UseDefaultServiceProvider((context, options) =>
    {
        var isDevelopment = context.HostingEnvironment.IsDevelopment();
        options.ValidateScopes = isDevelopment;
        options.ValidateOnBuild = isDevelopment;
    });

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, app.Environment);

try
{
    Log.Information(
        "Started {Application} in {Environment} mode.",
        hostEnvironment.ApplicationName,
        hostEnvironment.EnvironmentName);
    app.Run();
    Log.Information(
        "Stopped {Application} in {Environment} mode.",
        hostEnvironment.ApplicationName,
        hostEnvironment.EnvironmentName);
    return 0;
}
catch (Exception exception)
{
    Log.Fatal(
        exception,
        "{Application} terminated unexpectedly in {Environment} mode.",
        hostEnvironment.ApplicationName,
        hostEnvironment.EnvironmentName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static Logger CreateLogger(IHostEnvironment hostEnvironment, IConfiguration configuration) =>
    new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application",
            configuration["DOTNET_APPLICATIONNAME"] ?? hostEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", hostEnvironment.EnvironmentName)
        .CreateLogger();
