using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using BlazorPokemonCardSetViewer;
using BlazorPokemonCardSetViewer.Contracts;
using BlazorPokemonCardSetViewer.Pages.ViewModels;
using BlazorPokemonCardSetViewer.Services;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .CreateLogger();
    
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
    
    // Point to the Server project URL
    builder.Services.AddScoped(sp => new HttpClient 
    { 
        BaseAddress = new Uri("https://localhost:7240/") // Matches what's in the server terminal output
    });
    
    builder.Services.AddScoped<ICardService, CardService>();
    builder.Services.AddScoped<CardPageViewModel>();
    
    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Application start-up failed: {ex}");
}