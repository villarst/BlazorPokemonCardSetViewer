using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server;

public abstract class Program
{
    public static void Main (string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddDbContext<PokemonDbContext>(options =>
            options.UseSqlite("Data Source=Data/PokemonDb.db"));

        builder.Services.AddLogging();
        builder.Services.AddOpenApi();

        // Add CORS to allow your Blazor client to make requests
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("BlazorClientPolicy", policy =>
            {
                policy.WithOrigins("https://localhost:7292") // Client port number
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseCors("BlazorClientPolicy");
        app.MapControllers();
        app.Run();
    }
}