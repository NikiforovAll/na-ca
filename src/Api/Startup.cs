// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Api;

using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nikiforoval.CA.Template.Api.Filters;
using Nikiforoval.CA.Template.Api.Formatters.FluentValidation;
using Nikiforoval.CA.Template.Api.Services;
using Nikiforoval.CA.Template.Application;
using Nikiforoval.CA.Template.Application.SharedKernel.Interfaces;
using Nikiforoval.CA.Template.Infrastructure;

#pragma warning disable IDE0058 // Expression value is never used
internal class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        this.Configuration = configuration;
        this.Environment = environment;
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplication();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        services.AddInfrastructure(this.Configuration);

        services.AddCustomHealthChecks();

        services
            .AddHostingOptions()
            .AddCustomRouting();

        // Customize default API behavior
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = false);

        services
            .AddCustomApiVersioning()
            .AddCustomSwagger(this.Configuration)
            .AddHttpContextAccessor();

        services
            .AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddFluentValidation(options =>
                {
                    options.ValidatorOptions.PropertyNameResolver =
                        CamelCasePropertyNameResolver.ResolvePropertyName;
                    options.ValidatorOptions.DisplayNameResolver =
                        SplitPascalCaseDisplayNameResolver.ResolvePropertyName;
                });
    }

    /// <summary>
    /// Application pipeline
    /// </summary>
    /// <param name="app"></param>
    public void Configure(IApplicationBuilder app)
    {
        app.UseForwardedHeaders();
        app.UseCustomSerilogRequestLogging();

        app.UseStaticFiles()
            .UseDefaultFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapCustomHealthCheck(
                pattern: "/health",
                servicesPattern: "/health/ready");
        });

        app.UseCustomSwagger(this.Configuration);
    }
}

#pragma warning restore IDE0058 // Expression value is never used
