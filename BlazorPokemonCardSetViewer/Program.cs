using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using BlazorPokemonCardSetViewer;
using BlazorPokemonCardSetViewer.Pages.ViewModels;

// namespace BlazorPokemonCardSetViewer;
//
// public class Program
// {
//     private static void Main(string[] args)
//     {
//         Log.Logger = new LoggerConfiguration()
//             .MinimumLevel.Debug()
//             .WriteTo.File("logs\\log-.txt", rollingInterval: RollingInterval.Day)
//             .CreateLogger();
//
//         try
//         {
//             var builder = WebAssemblyHostBuilder.CreateDefault(args);
//             builder.AddSingleton<CounterViewModel>;
//             
//         }
//         catch (Exception ex)
//         {
//             Log.Logger.Error(ex, "An error occured, the application will exit.");
//         }
//     }
// }
//

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();