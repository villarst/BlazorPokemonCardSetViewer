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
        .WriteTo.File("logs\\log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
    
    // Point to the Server project URL
    builder.Services.AddScoped(sp => new HttpClient 
    { 
        BaseAddress = new Uri("http://localhost:5205/") // Matches what's in the server terminal output
    });
    
    builder.Services.AddScoped<IWeatherService, WeatherService>();
    builder.Services.AddScoped<ICardService, CardService>();
    builder.Services.AddScoped<WeatherViewModel>();
    builder.Services.AddScoped<CounterViewModel>();
    builder.Services.AddScoped<CardPageViewModel>();
    
    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Error");
}