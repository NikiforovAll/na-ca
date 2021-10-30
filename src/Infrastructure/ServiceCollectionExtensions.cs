// Copyright (c) Oleksii Nikiforov, 2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace Nikiforoval.CA.Template.Infrastructure;

using System.Linq;
using System.Text.Json;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nikiforoval.CA.Template.Application.Interfaces;
using Nikiforoval.CA.Template.Application.SharedKernel.Interfaces;
using Nikiforoval.CA.Template.Infrastructure.Persistence;
using Npgsql;
using Npgsql.TypeHandlers;
using Npgsql.TypeMapping;
using NpgsqlTypes;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }

    private static void OverrideNpgsqlGlobalMappings(JsonSerializerOptions serializerOptions)
    {
        var origJsonbMapping = NpgsqlConnection.GlobalTypeMapper
            .Mappings
            .Single(m => m.NpgsqlDbType == NpgsqlDbType.Jsonb);

        NpgsqlConnection.GlobalTypeMapper
            .RemoveMapping(origJsonbMapping.PgTypeName);
        NpgsqlConnection.GlobalTypeMapper
            .AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = origJsonbMapping.PgTypeName,
                NpgsqlDbType = origJsonbMapping.NpgsqlDbType,
                DbTypes = origJsonbMapping.DbTypes,
                ClrTypes = origJsonbMapping.ClrTypes,
                InferredDbType = origJsonbMapping.InferredDbType,
                TypeHandlerFactory = new JsonbHandlerFactory(serializerOptions)
            }.Build());
    }
}
