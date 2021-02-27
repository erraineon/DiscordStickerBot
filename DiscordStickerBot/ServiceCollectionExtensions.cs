using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiscordStickerBot
{
    public static class ServiceCollectionExtensions
    {
        public static TOptions BindConfigurationSection<TOptions>(
            this IServiceCollection services,
            IConfiguration configuration) where TOptions : class
        {
            var options = (TOptions) FormatterServices.GetUninitializedObject(typeof(TOptions));
            configuration.GetSection(typeof(TOptions).Name).Bind(options);
            services.TryAddSingleton(options);
            return options;
        }
    }
}