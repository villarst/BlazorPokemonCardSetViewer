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
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
    
            builder.Services.AddScoped(sp => new HttpClient 
            { 
                BaseAddress = new Uri("https://localhost:7240/") // Server port number
            });
    
            builder.Services.AddScoped<ICardsService, CardsService>();
            builder.Services.AddScoped<CardPageViewModel>();
    
            await builder.Build().RunAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Application start-up failed: {ex}");
        }
    }
}





