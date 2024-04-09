using Microsoft.AspNetCore.Identity;
using MR.Authentication.Data;
using MR.Authentication.Extensions;
using MR.WebApi.Core.Identity;

namespace MR.Authentication.Configuration;

public static class IdentityConfig
{
    public static void AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        //Identity
        services.AddDefaultIdentity<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddErrorDescriber<IdentityMensagensPortugues>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddJwtConfiguration(configuration);
    }

    public static void UseIdentityConfiguration(this IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        app.UseAuthentication();
        app.UseAuthorization();
    }
}
