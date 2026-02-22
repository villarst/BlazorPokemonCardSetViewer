using BlazorPokemonCardSetViewer.Pages.ViewModels;
using BlazorPokemonCardSetViewer.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;

namespace BlazorPokemonCardSetViewer;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
    
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Logging.SetMinimumLevel(LogLevel.Error);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
    
            builder.Services.AddScoped(_ => new HttpClient 
            { 
                BaseAddress = new Uri("https://localhost:7240/") // Server port number
            });
    
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<ISetService, SetService>();
            builder.Services.AddScoped<ICardPageViewModel, CardPageViewModel>();
    
            await builder.Build().RunAsync();
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Application start-up failed: {ex}");
        }
    }
}





