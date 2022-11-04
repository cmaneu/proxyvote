using Spectre.Console;
namespace TestDataGenerator;

public static class TestProxyApplicationConsoleExtensions
{
    public static void DisplayAsConsoleTable(this IEnumerable<TestProxyApplication> applications, string? tableTitle)
    {
        var table = new Table();
        if(!string.IsNullOrWhiteSpace(tableTitle))
        {
            table.Title = new TableTitle(tableTitle);
        }
        table.AddColumns("Partition", "RegistrationId", "Applicant");

        foreach (var application in applications)
        {
            table.AddRow(
                application.PartitionKey,
                application.RegistrationId,
                application.Applicant.FirstName + " " + application.Applicant.LastName);
        }
        AnsiConsole.Write(table);
    }
}