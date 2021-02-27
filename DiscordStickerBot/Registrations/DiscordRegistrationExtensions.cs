using Discord.WebSocket;
using DiscordStickerBot.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordStickerBot.Registrations
{
    public static class DiscordRegistrationExtensions
    {
        public static IHostBuilder UseDiscord(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureServices(
                    (hostingContext, services) =>
                    {
                        var discordOptions =
                            services.BindConfigurationSection<DiscordOptions>(hostingContext.Configuration);
                        if (discordOptions.Enabled)
                        {
                            services.AddSingleton(new DiscordSocketClient());
                            services.AddHostedService<DiscordClientLauncher>();
                        }
                    });
        }
    }
}