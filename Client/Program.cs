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

    builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    builder.Services.AddScoped<IWeatherService, WeatherService>();
    builder.Services.AddScoped<WeatherViewModel>();
    builder.Services.AddScoped<CounterViewModel>();
    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Error");
}



