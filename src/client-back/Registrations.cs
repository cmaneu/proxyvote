using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

using ProxyVote.Core;

namespace ProxyVote.Client.Back
{
    public static class Registrations
    {
        [FunctionName(nameof(SubmitRegistration))]
        [OpenApiOperation(nameof(SubmitRegistration), tags: new[] { "registration" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ProxyRegistration), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> SubmitRegistration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registration")] ProxyRegistration registration,
            
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = registration.Id;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}

