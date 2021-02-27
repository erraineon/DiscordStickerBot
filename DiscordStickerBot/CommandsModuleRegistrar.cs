using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Hosting;

namespace DiscordStickerBot
{
    public class CommandsModuleRegistrar : IHostedService
    {
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;

        public CommandsModuleRegistrar(
            CommandService commandService,
            IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var module in _commandService.Modules)
                await _commandService.RemoveModuleAsync(module);
        }
    }
}