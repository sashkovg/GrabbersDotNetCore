using System.IO;
using GtabbersDotNetCore.Bll.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GtabbersDotNetCore.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Helper.ConfigurationGrabber = builder.Build();

            Log.Logger = new LoggerConfiguration()
                 .ReadFrom.Configuration(Helper.ConfigurationGrabber)
                 .CreateLogger();
            

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, config) =>
            {
                config.ClearProviders();
            }).UseSerilog().UseStartup<Startup>().Build();
    }
}
