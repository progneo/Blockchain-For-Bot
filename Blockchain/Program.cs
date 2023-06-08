using Blockchain.Miner;
using Blockchain.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blockchain;

public static class Program
{
    private const string AppSettingsFile = "appsettings.json";

    private static async Task Main()
    {
        var builder = new HostBuilder();

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile(AppSettingsFile, false, true);
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices((hostingContext, services) =>
        {
            services.AddSingleton<TransactionPool>();
            services.AddSingleton<BlockMiner>();
            services.AddHostedService<WebServerService>();
        });

        builder.ConfigureLogging((context, loggingBuilder) => { loggingBuilder.AddConsole(); });

        await builder.RunConsoleAsync();
    }
}