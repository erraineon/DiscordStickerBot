using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordStickerBot.Options;

namespace DiscordStickerBot
{
    public class CommandParser
    {
        private readonly CommandService _commandService;
        private readonly DiscordOptions _discordOptions;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IServiceProvider _serviceProvider;

        public CommandParser(
            DiscordSocketClient discordSocketClient,
            CommandService commandService,
            DiscordOptions discordOptions,
            IServiceProvider serviceProvider)
        {
            _discordSocketClient = discordSocketClient;
            _commandService = commandService;
            _discordOptions = discordOptions;
            _serviceProvider = serviceProvider;
        }

        public async Task TryExecute(SocketMessage message)
        {
            if (message is IUserMessage userMessage &&
                message.Author is IUser {IsBot: false} &&
                message.Content.StartsWith(_discordOptions.Prefix))
            {
                var input = userMessage.Content[_discordOptions.Prefix.Length..];
                var commandContext = new CommandContext(_discordSocketClient, userMessage);
                var result = await _commandService.ExecuteAsync(commandContext, input, _serviceProvider);
                if (result.Error == CommandError.Exception) throw new Exception(result.ErrorReason);
            }
        }
    }
}