using Microsoft.AspNetCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

namespace SportsComplex.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogger();
            try
            {
                Log.Information("Starting SportsComplex API host.");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "SportsComplex API host terminated unexpectedly.");
            }
            finally
            {
                Log.Information("SportsComplex API stopped.");
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseConfiguration(Configuration)
                .UseSerilog()
                .Build();

        private static IConfiguration Configuration
        {
            get
            {
                var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");
                if (string.IsNullOrEmpty(environmentName))
                {
                    throw new Exception("Environment is not set");
                }

                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environmentName);

                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
            }
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new [] {new DbUpdateExceptionDestructurer()}))
                .CreateLogger();
        }
    }
}