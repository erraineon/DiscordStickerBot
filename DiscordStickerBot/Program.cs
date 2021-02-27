using DiscordStickerBot.Registrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordStickerBot
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(
                    config => config
                        .AddJsonFile("appsettings.json"))
                .UseDiscord()
                .UseTelegram()
                .UseCommands()
                .ConfigureLogging(logging => logging.AddSimpleConsole())
                .UseConsoleLifetime();
            var host = hostBuilder.Build();
            host.Run();
        }
    }
}