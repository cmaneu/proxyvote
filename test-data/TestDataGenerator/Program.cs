using Bogus;using Bogus.DataSets;

using CountryData;
using CountryData.Bogus;

using Microsoft.Azure.Cosmos;

using ProxyVote.Core.Entities;
using ProxyVote.IdentityAuthority.Core;

using TestDataGenerator;

Console.WriteLine("ProxyVote - Data Generator");

// Setup Bogus
var locale = args.Length >= 3 ? args[2] : "fr"; 
var names = new Bogus.DataSets.Name(locale);



var addresses = new Bogus.DataSets.Address(locale);






// Setup fakers
var fakeProxy = new Faker<ProxyVoter>(locale)
        .RuleFor(r => r.FirstName, f => names.FirstName())
        .RuleFor(r => r.LastName, f => names.LastName())
        .RuleFor(r => r.BirthDate, f => f.Date.Between(DateTime.Now.AddYears(-80), DateTime.Now.AddYears(-18)))
    ;

var fakeApplicant = new Faker<Applicant>(locale)
        .CustomInstantiator(f =>
        {
            var address = f.Person.Address;
            return new Applicant()
            {
                CityName = address.City, 
                PostalCode = address.ZipCode,
                State = address.State
            };

        })
        .RuleFor(r => r.FirstName, f => names.FirstName())
        .RuleFor(r => r.LastName, f => names.LastName())
        .RuleFor(r => r.EmailAddress, (f, a) => f.Internet.Email(a.FirstName, a.LastName))
        .RuleFor(r => r.BirthDate, f => f.Date.Between(DateTime.Now.AddYears(-80), DateTime.Now.AddYears(-18)))
        .RuleFor(r => r.StreetAddress, f => f.Address.StreetAddress())
    ;

var fakeRegistration = new Faker<TestProxyApplication>(locale)
    .RuleFor(r => r.CreatedAt, f => f.Date.Between(DateTime.Now.AddDays(-60), DateTime.Now.AddDays(-1)))
    .RuleFor(r => r.ValidUntil, f => f.Date.Between(DateTime.Now.AddDays(90), DateTime.Now.AddDays(120)))
    .RuleFor(r => r.RegistrationId, f => f.Random.Guid().ToString())
    .RuleFor(r => r.Applicant, f => fakeApplicant)
    .RuleFor(r => r.ProxyVoter, f => fakeProxy)
    ;

////////////////////////////////////////////////
////// Generate applications 

CosmosClient client = new CosmosClient(args[0], args[1]);
await client.CreateDatabaseIfNotExistsAsync("proxyvote");
var database = client.GetDatabase("proxyvote");
Container container = await database.CreateContainerIfNotExistsAsync(
    "Applications",
    "/PartitionKey",
    400);

Container regContainer = await database.CreateContainerIfNotExistsAsync(
    "Registrations",
    "/PartitionKey",
    400);


RegistrationIdentityService registrationIdentityService = new RegistrationIdentityService(args[0], args[1]);

var random = new Random();


for (int i = 0; i < 100000; i++)
{
    var fakeItem = fakeRegistration.Generate();
    ItemResponse<TestProxyApplication> createResponse = await container.CreateItemAsync(fakeItem);

    // Validate some applications
    if (i % 100 == 1)
    {
        Console.WriteLine(i);
        var validation = new ApplicationValidation()
        {
            Type = "test",
            ValidationData = "na",
            ValidationTime = fakeItem.CreatedAt.Value.AddDays(1).AddDays(random.Next(0, 15)),
            ValidatorId = random.Next(10000, 12000).ToString()
        };

        await registrationIdentityService.ValidateRegistration("2022-" + fakeItem.Applicant.PostalCode[..2],
            fakeItem.RegistrationId, validation);

    }

}