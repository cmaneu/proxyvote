﻿using System;
using System.IO;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ProxyVote.Client.Back;
using ProxyVote.Core.Adapters;
using ProxyVote.Core.Infrastructure;
using ProxyVote.Core.Services;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ProxyVote.Client.Back;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        FunctionsHostBuilderContext context = builder.GetContext();
        builder.ConfigurationBuilder
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: false,
                reloadOnChange: true)
            //Adding Environmental Files //add Conditions
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"),
                optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDbContextFactory<ProxyDBContext>(
            (IServiceProvider sp, DbContextOptionsBuilder opts) =>
            {
                var configuration = builder.GetContext().Configuration;
                CosmosSettings cosmosSettings =
                    (CosmosSettings)configuration.GetSection("CosmosDb").Get(typeof(CosmosSettings));

                opts.UseCosmos(
                        cosmosSettings.EndPoint,
                        cosmosSettings.AccessKey,
                        "ProxyVote");
                    //.LogTo(Console.WriteLine);
            });


        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IProxyDbProvider, ProxyCosmosDbProvider>();
        builder.Services.AddScoped<ProxyRegistrationService>();
    }
}