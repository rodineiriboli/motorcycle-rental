using Microsoft.EntityFrameworkCore;
using MR.Authentication.Data;

namespace MR.Authentication.Configuration;

public static class DatabaseConfig
{
    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }
}
