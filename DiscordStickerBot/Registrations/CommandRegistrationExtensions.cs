using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordStickerBot.Registrations
{
    public static class CommandRegistrationExtensions
    {
        public static IHostBuilder UseCommands(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureServices(services => services
                    .AddSingleton(new CommandService(new CommandServiceConfig {DefaultRunMode = RunMode.Sync}))
                    .AddHostedService<CommandsModuleRegistrar>()
                    .AddHostedService<ChatClientEventDispatcher>()
                    .AddTransient<CommandParser>());
        }
    }
}