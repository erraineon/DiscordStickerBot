using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordStickerBot.Options;
using Microsoft.Extensions.Hosting;

namespace DiscordStickerBot
{
    public class DiscordClientLauncher : IHostedService
    {
        private readonly DiscordOptions _discordOptions;
        private readonly DiscordSocketClient _discordSocketClient;

        public DiscordClientLauncher(
            DiscordSocketClient discordSocketClient,
            DiscordOptions discordOptions)
        {
            _discordSocketClient = discordSocketClient;
            _discordOptions = discordOptions;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var discordReady = new TaskCompletionSource();

            Task OnReady()
            {
                _discordSocketClient.Ready -= OnReady;
                discordReady.SetResult();
                return Task.CompletedTask;
            }

            _discordSocketClient.Ready += OnReady;
            await _discordSocketClient.LoginAsync(TokenType.Bot, _discordOptions.Token);
            await _discordSocketClient.StartAsync();
            await discordReady.Task;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordSocketClient.StopAsync();
        }
    }
}