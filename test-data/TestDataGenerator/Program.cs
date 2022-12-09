using Bogus;
using Bogus.DataSets;
using CountryData;
using CountryData.Bogus;
using Spectre.Console;
using Microsoft.Azure.Cosmos;
using System.Linq;
using ProxyVote.Core.Entities;
using ProxyVote.IdentityAuthority.Core;
using TestDataGenerator;

// Arg0 = HTTP Endpoint
// Arg1 = Account key
// Arg2 = locale
// Arg3 = # of records to generate

AnsiConsole.Write(
    new FigletText("ProxyVote")
        .LeftAligned()
        .Color(Color.Blue));
AnsiConsole.MarkupLine("[bold]ProxyVote Test Data Generator[/]");

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
                //PostalCode = "75015", // Used to create a hot partition
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

var numberOfApplicationsToGenerate = args.Count() >= 4 ? Convert.ToInt32(args[3]) : 10000;

var batchSize = 100;
var percentageOfRegistrationsToValidate= 2;

// Create application with batch of 100 items size
// Save some of them for validation

AnsiConsole.MarkupLineInterpolated($"[blue]Generating {numberOfApplicationsToGenerate} applications[/]");

// Asynchronous

    await AnsiConsole.Progress()
    .Columns(new ProgressColumn[]
    {
        new TaskDescriptionColumn(),
        new ProgressBarColumn(),
        new PercentageColumn(),
        new RemainingTimeColumn(),
        new SpinnerColumn(Spinner.Known.Earth)
    })
    .StartAsync(async ctx =>
    {
        // Define tasks
        var task1 = ctx.AddTask("[green]Global progress:[/]");
        task1.MaxValue = numberOfApplicationsToGenerate;
        var task2 = ctx.AddTask("      [blue]Generating items[/]");
        var task3 = ctx.AddTask("      [blue]Uploading new applications to Cosmos[/]");
        var task4 = ctx.AddTask("      [blue]Validating registrations[/]");


        // One batch
        for(int i = 0; i < numberOfApplicationsToGenerate; i+= batchSize)
        {
            List<double> consumedRU = new List<double>(batchSize);
            task2.Value = 0;
            task3.Value = 0;
            task4.Value = 0;


            // Generate batchSize items
            var fakeRegistrations = fakeRegistration.Generate(batchSize).ToList();
            // Pick random items to validate
            var fakeRegistrationToValidate = fakeRegistrations.OrderBy(x => random.Next()).Take(10).ToList();
            task2.Value = 100;

            // Send them to Cosmos
            task3.MaxValue = batchSize;
            foreach (var reg in fakeRegistrations)
            {
                ItemResponse<TestProxyApplication> createResponse = await container.CreateItemAsync(reg);
                consumedRU.Add(createResponse.RequestCharge);
                task3.Increment(1);
            }

             AnsiConsole.MarkupLineInterpolated ($"Average Consumed RU: {consumedRU.Average().ToString()}");
            

            // Validate items
            task4.MaxValue = fakeRegistrationToValidate.Count();
            AnsiConsole.MarkupLineInterpolated($"[blue]Validating {fakeRegistrationToValidate.Count()} registrations[/]");
            foreach (var reg in fakeRegistrationToValidate)
            {
                var validation = new ApplicationValidation()
                {
                    Type = "test",
                    ValidationData = "na",
                    ValidationTime = reg.CreatedAt.Value.AddDays(1).AddDays(random.Next(0, 15)),
                    ValidatorId = random.Next(10000, 12000).ToString()
                };

                await registrationIdentityService.ValidateRegistration("2022-" + reg.Applicant.PostalCode[..2],
                    reg.RegistrationId, validation);
                task4.Increment(1);
            }
            fakeRegistrationToValidate.DisplayAsConsoleTable("Applications validated");
            task1.Increment(batchSize);
        }
    });
       





// /// Old version below
// return;
// for (int i = 0; i < 100000; i++)
// {
//     var fakeItem = fakeRegistration.Generate();
//     ItemResponse<TestProxyApplication> createResponse = await container.CreateItemAsync(fakeItem);

//     // Validate some applications
//     if (i % 100 == 1)
//     {
//         Console.WriteLine(i);
//         var validation = new ApplicationValidation()
//         {
//             Type = "test",
//             ValidationData = "na",
//             ValidationTime = fakeItem.CreatedAt.Value.AddDays(1).AddDays(random.Next(0, 15)),
//             ValidatorId = random.Next(10000, 12000).ToString()
//         };

//         await registrationIdentityService.ValidateRegistration("2022-" + fakeItem.Applicant.PostalCode[..2],
//             fakeItem.RegistrationId, validation);

//     }

// }