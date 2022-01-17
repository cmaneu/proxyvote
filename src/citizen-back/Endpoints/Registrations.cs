using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

using ProxyVote.Core.Entities;
using ProxyVote.Core.Services;

namespace ProxyVote.Citizen.Back.Endpoints;

public class Registrations
{
    private readonly ProxyRegistrationService _registrationService;

    public Registrations(ProxyRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [FunctionName(nameof(SubmitRegistration))]
    [OpenApiOperation(nameof(SubmitRegistration), tags: new[] { "registration" }, Description = "Insert a new proxy registration in the system.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ProxyRegistration), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> SubmitRegistration(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registration")] ProxyRegistration registration,
        ILogger log)
    {
        log.LogInformation($"{nameof(SubmitRegistration)} called.");

        var registrationId = await _registrationService.CreateRegistrationAsync(registration);
        
        return new CreatedResult("/registrations", new { Id = registrationId });
    }

    [FunctionName(nameof(TestRegistration))]
    [OpenApiOperation(nameof(TestRegistration), tags: new[] { "registration" }, Description = "Test.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProxyRegistration), Description = "The OK response")]
    public IActionResult TestRegistration(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registrationtest")] ProxyRegistration registration,
    ILogger log)
    {
        log.LogInformation($"{nameof(TestRegistration)} called.");

        return new OkObjectResult(new ProxyRegistration()
        {
            RegistrationId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            ValidUntil = DateTime.UtcNow.AddDays(200),
            Applicant = new Applicant()
            {
                FirstName = "Jean",
                LastName = "Valjean",
                BirthDate = DateTime.UtcNow.AddYears(-35),
                EmailAddress = "demo@demo.com",
                CityName = "Paris",
                PostalCode = "75001",
                State = "Ile de France",
                StreetAddress = "39 Quai du Président Roosevelt"
            },

            ProxyVoter = new ProxyVoter()
            {
                FirstName = "Henri",
                LastName = "Dole",
                BirthDate = DateTime.UtcNow.AddYears(-42),
            }
            
        });
    }
}

