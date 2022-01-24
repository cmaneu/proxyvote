using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using ProxyVote.Core.Configuration;

namespace ProxyVote.Citizen.Back.Endpoints;

public static class Configuration
{
    [FunctionName("Configuration")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "config/client-config.json")] HttpRequest req,
        ExecutionContext context,
        ILogger log)
    {
        // TODO: ugly, and not using DI
        var config = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile(Path.Combine(context.FunctionAppDirectory, "settings.json"), optional: false, reloadOnChange: true)
            .AddJsonFile(Path.Combine(context.FunctionAppDirectory, $"local.settings.json"), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        CitizenClientConfiguration clientConfig  = (CitizenClientConfiguration)config.Build().GetSection("CitizenClient").Get(typeof(CitizenClientConfiguration));

        return new OkObjectResult(clientConfig);
    }
}
