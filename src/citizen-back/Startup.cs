﻿using System;
using System.IO;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ProxyVote.Citizen.Back;
using ProxyVote.Core.Adapters;
using ProxyVote.Core.Infrastructure;
using ProxyVote.Core.Services;
using ProxyVote.IdentityAuthority.Core;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ProxyVote.Citizen.Back;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        FunctionsHostBuilderContext context = builder.GetContext();
        builder.ConfigurationBuilder
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, "settings.json"), optional: false, reloadOnChange: true)
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"local.settings.json"), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;
        CosmosSettings cosmosSettings = (CosmosSettings)configuration.GetSection("CosmosDb").Get(typeof(CosmosSettings));

        builder.Services.AddDbContextFactory<ProxyDBContext>(
            (IServiceProvider sp, DbContextOptionsBuilder opts) =>
            {
      
                opts.UseCosmos(
                        cosmosSettings.EndPoint,
                        cosmosSettings.AccessKey,
                        "proxyvote");
                    //.LogTo(Console.WriteLine);
            });


        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IProxyDbProvider, ProxyCosmosDbProvider>();
        builder.Services.AddScoped<ProxyRegistrationService>();
        builder.Services.AddScoped<RegistrationIdentityService>(provider => new RegistrationIdentityService(cosmosSettings.EndPoint, cosmosSettings.AccessKey));
    }
}