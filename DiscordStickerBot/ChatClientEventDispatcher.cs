using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordStickerBot
{
    public class ChatClientEventDispatcher : IHostedService
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger<ChatClientEventDispatcher> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ChatClientEventDispatcher(
            DiscordSocketClient discordSocketClient,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ChatClientEventDispatcher> logger)
        {
            _discordSocketClient = discordSocketClient;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived += OnMessageReceived;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived -= OnMessageReceived;
            return Task.CompletedTask;
        }

        private Task OnMessageReceived(SocketMessage message)
        {
            ParseMessageAsyncVoid(message);
            return Task.CompletedTask;
        }

        private async void ParseMessageAsyncVoid(SocketMessage message)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var commandParser = scope.ServiceProvider.GetRequiredService<CommandParser>();
                await commandParser.TryExecute(message);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, $"error while handling message {message}");
            }
        }
    }
}