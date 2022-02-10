using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProxyVote.Citizen.Front;
using ProxyVote.Citizen.Front.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient ());

builder.Services.AddScoped<ProxyAPIService>();

var host = builder.Build();

var proxyApiService = host.Services.GetRequiredService<ProxyAPIService>();
await proxyApiService.InitializeService(builder.HostEnvironment.BaseAddress);

await host.RunAsync();
