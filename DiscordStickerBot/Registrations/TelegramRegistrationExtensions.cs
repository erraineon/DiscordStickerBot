using DiscordStickerBot.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace DiscordStickerBot.Registrations
{
    public static class TelegramRegistrationExtensions
    {
        public static IHostBuilder UseTelegram(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureServices(
                    (hostingContext, services) =>
                    {
                        var telegramOptions =
                            services.BindConfigurationSection<TelegramOptions>(hostingContext.Configuration);
                        if (telegramOptions.Enabled)
                        {
                            var telegramBotClient = new TelegramBotClient(telegramOptions.Token);
                            services.AddSingleton<ITelegramBotClient>(telegramBotClient);
                        }
                    });
        }
    }
}