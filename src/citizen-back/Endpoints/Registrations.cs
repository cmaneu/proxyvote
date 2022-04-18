using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

using ProxyVote.Core.Entities;
using ProxyVote.Core.Services;
using ProxyVote.IdentityAuthority.Core;

namespace ProxyVote.Citizen.Back.Endpoints;

public class Registrations
{
    private readonly ProxyRegistrationService _registrationService;
    private readonly RegistrationIdentityService _registrationIdentityService;

    public Registrations(ProxyRegistrationService registrationService, RegistrationIdentityService registrationIdentityService)
    {
        _registrationService = registrationService;
        _registrationIdentityService = registrationIdentityService;
    }

    [FunctionName(nameof(SubmitRegistration))]
    [OpenApiOperation(nameof(SubmitRegistration), tags: new[] { "application" }, Description = "Insert a new proxy application in the system.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ProxyApplication), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> SubmitRegistration(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "application")] ProxyApplication application,
        ILogger log)
    {
        log.LogInformation($"{nameof(SubmitRegistration)} called.");

        var registrationId = await _registrationService.CreateRegistrationAsync(application);
        
        return new CreatedResult("/registrations", new { Id = registrationId });
    }


    [FunctionName(nameof(GetRegistrationById))]
    [OpenApiOperation(nameof(GetRegistrationById), tags: new[] { "application" }, Description = "Get a application by Id.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProxyApplication), Description = "The requested application")]
    public async Task<IActionResult> GetRegistrationById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "application/{department}/{registrationId}")]
        HttpRequest req,
        string registrationId,
        string department,
        ILogger log)
    {
        var registration = await _registrationIdentityService.GetRegistrationById(department, registrationId);
        return registration == null ? new NotFoundResult() : new OkObjectResult(registration);
    }


    [FunctionName(nameof(TestRegistration))]
    [OpenApiOperation(nameof(TestRegistration), tags: new[] { "application" }, Description = "Test.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ProxyApplication), Description = "The OK response")]
    public IActionResult TestRegistration(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registrationtest")] ProxyApplication application,
    ILogger log)
    {
        log.LogInformation($"{nameof(TestRegistration)} called.");

        return new OkObjectResult(new ProxyApplication()
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

