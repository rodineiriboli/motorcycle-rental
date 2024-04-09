using MR.Authentication.Configuration;

namespace MR.Authentication;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();


        // Add services to the container.
        builder.Services.AddControllers();

        // Database
        builder.Services.AddDatabaseConfiguration(builder.Configuration);

        // Identity
        builder.Services.AddIdentityConfiguration(builder.Configuration);

        // Swagger Config
        builder.Services.AddSwaggerConfiguration();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerSetup();
        }

        app.UseHttpsRedirection();

        // Identity
        app.UseIdentityConfiguration();

        app.MapControllers();

        app.Run();
    }
}
