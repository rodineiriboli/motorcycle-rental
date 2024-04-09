using Microsoft.OpenApi.Models;

namespace MR.Authentication.Configuration;

public static class SwaggerConfig
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Motorcycle Rental - Identity API",
                Description = "Microsserviço de identidade - Mottu",
                Contact = new OpenApiContact() { Name = "Rodinei Riboli", Url = new Uri("https://www.linkedin.com/in/rodineiriboli") },
                License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licences/MIT") }
            });

            //x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    Description = "Insira o token seguindo este padrão: Bearer {token}",
            //    Name = "Autorization",
            //    Scheme = "Bearer",
            //    BearerFormat = "JWT",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey
            //});
        });
    }

    public static void UseSwaggerSetup(this IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
    }
}
