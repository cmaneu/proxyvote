using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace ProxyVote.Citizen.Back
{
    public class ProxyVoteCitizenOpenAPIConfiguration : IOpenApiConfigurationOptions
    {
        public OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = "0.1.0",
            Title = "ProxyVote Citizen APIs",
            Description = "APIs used",
            Contact = new OpenApiContact()
            {
                Name = "ProxyVote",
                Email = "proxyvote@maneu.org"
            }
        };

        public List<OpenApiServer> Servers
        {
            get
            {
                return (new OpenApiServer[] { 
                    new OpenApiServer { Url = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME") },
                    new OpenApiServer { Url = "preprod-proxvote.azurewebsites.net" },
                    new OpenApiServer { Url = "prod-proxvote.azurewebsites.net" }
                }
                
                ).ToList();
            }
            set => throw new NotImplementedException();
        }

        OpenApiVersionType IOpenApiConfigurationOptions.OpenApiVersion { get => OpenApiVersionType.V3; set => throw new NotImplementedException(); }
        bool IOpenApiConfigurationOptions.IncludeRequestingHostName { get => true; set => throw new NotImplementedException(); }
        bool IOpenApiConfigurationOptions.ForceHttp { get => true; set => throw new NotImplementedException(); }
        bool IOpenApiConfigurationOptions.ForceHttps { get => false; set => throw new NotImplementedException(); }
    }
}
