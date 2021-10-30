// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Api;


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nikiforoval.CA.Template.Infrastructure.Persistence;
using NSwag;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures the ForwardedHeadersOptions.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The services with options services added.</returns>
    public static IServiceCollection AddHostingOptions(
        this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;
            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are enabled by explicit
            // configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
        return services;
    }

    /// <summary>
    /// Adds custom routing settings which determines how URL's are generated.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>The services with routing services added.</returns>
    public static IServiceCollection AddCustomRouting(this IServiceCollection services) =>
        services.AddRouting(options => options.LowercaseUrls = true);

    /// <summary>
    /// Adds custom versioning settings and format.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns></returns>
    public static IServiceCollection AddCustomApiVersioning(this IServiceCollection services) => services
        .AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
        })
        .AddVersionedApiExplorer(x => x.GroupNameFormat = "'v'VVV"); // Version format: 'v'major[.minor][-status]

    /// <summary>
    /// Adds Swagger services and configures the Swagger services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration"></param>
    /// <returns>The services with Swagger services added.</returns>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<FluentValidationSchemaProcessor>();

        var apiVersionProvider = services.BuildServiceProvider()
            .GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var apiVersionDescription in apiVersionProvider.ApiVersionDescriptions)
        {
            services.AddOpenApiDocument((options, serviceProvider) =>
            {
                options.Version = apiVersionDescription.ApiVersion.ToString();
                options.PostProcess = document =>
                {
                    document.Info.Title = "Clean Architecture API";
                    document.Info.Description = "";
                };

                // TODO: add strongly typed options
                var identityProviderUrl = configuration.GetValue<string>("IdentityProvider:ExternalUrl");
                var tokenUrl = $"{identityProviderUrl}{configuration.GetValue<string>("IdentityProvider:TokenEndpoint")}";
                var authorizationEndpoint = $"{identityProviderUrl}{configuration.GetValue<string>("IdentityProvider:AuthorizationEndpoint")}";

                if (identityProviderUrl is not null)
                {
                    identityProviderUrl = identityProviderUrl.EndsWith("/", StringComparison.InvariantCultureIgnoreCase)
                    ? identityProviderUrl
                    : $"{identityProviderUrl}/";
                    options.AddSecurity("oauth2", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.OAuth2,
                        Description = "OAuth2 Client Authorization",
                        Flow = OpenApiOAuth2Flow.Implicit,
                        TokenUrl = tokenUrl,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                Scopes = new Dictionary<string, string>(),
                                AuthorizationUrl = authorizationEndpoint,
                            },
                        }
                    });
                    // Manually provide generated JWT
                    options.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: Bearer {your JWT token}."
                    });
                }

                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("oauth2"));
                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));

                var fluentValidationSchemaProcessor = serviceProvider
                    .GetService<FluentValidationSchemaProcessor>();
                options.SchemaProcessors.Add(fluentValidationSchemaProcessor);
            });
        }
        return services;
    }

    /// <summary>
    /// Adds cross-origin resource sharing (CORS) services and configures named CORS policies. See
    /// https://docs.asp.net/en/latest/security/cors.html
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="policyName"></param>
    /// <param name="configuration"></param>
    /// <returns>The services with CORS services added.</returns>
    public static IServiceCollection AddCustomCors(this IServiceCollection services, string policyName, IConfiguration configuration)
    {
        // Create named CORS policies here which you can consume using application.UseCors("PolicyName")
        // or a [EnableCors("PolicyName")] attribute on your controller or action.

        var origins = configuration.GetSection("AllowedOrigins").Get<string[]>();
        return services.AddCors(builder =>
            builder.AddPolicy(policyName,
                x => x
                    .WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("content-disposition")));
    }

    /// <summary>
    /// Adds health check to the application.
    /// </summary>
    /// <param name="services">The services. </param>
    /// <returns>The services with health check services added.</returns>
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        // Add health checks for external dependencies here.
        // See https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks

        services
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
                name: "Database",
                failureStatus: HealthStatus.Degraded,
                tags: new string[] { "services" });

        return services;
    }
}
