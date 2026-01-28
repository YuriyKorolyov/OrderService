using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderService.DataAccess.Postgres;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Priority:
        // 1) ConnectionStrings:DefaultConnection from config files
        // 2) env var "ConnectionStrings__DefaultConnection"
        // 3) env var "DefaultConnection"
        var configuration = BuildConfiguration();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? configuration["ConnectionStrings:DefaultConnection"]
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found. " +
                "Add it to OrderService.WebApi/appsettings.json or set env var ConnectionStrings__DefaultConnection.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        // We try to load appsettings from OrderService.WebApi (typical startup project),
        // regardless of which folder dotnet-ef is executed from.
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        DirectoryInfo? root = current;

        while (root is not null && !File.Exists(Path.Combine(root.FullName, "OrderService.slnx")))
        {
            root = root.Parent;
        }

        var solutionRoot = root?.FullName ?? current.FullName;
        var webApiDir = Path.Combine(solutionRoot, "OrderService.WebApi");

        var builder = new ConfigurationBuilder()
            .SetBasePath(webApiDir)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}

