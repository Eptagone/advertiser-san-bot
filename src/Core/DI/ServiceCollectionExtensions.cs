using System.Reflection;
using App.Core.Configuration;
using App.Core.Exceptions;
using App.Core.Resources;
using App.Core.Services;
using App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Telegram.BotAPI;

namespace App.Core.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBetterLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.TryAddSingleton<BetterStringLocalizerFactory>();
        services.TryAddTransient(typeof(IBetterStringLocalizer<>), typeof(BetterStringLocalizer<>));
        return services;
    }

    /// <summary>
    /// Add telegram bot services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddTelegramBot(
        this IServiceCollection services,
        Action<BotSettings> configure
    )
    {
        services.Configure(configure);

        // Configure the Telegram bot client
        services
            .AddHttpClient(
                nameof(TelegramBotClient),
                (provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<BotSettings>>().Value;
                    client.BaseAddress = new Uri(
                        string.IsNullOrEmpty(options.ServerAddress)
                            ? TelegramBotClientOptions.BaseServerAddress
                            : options.ServerAddress
                    );
                }
            )
            .RemoveAllLoggers();
        services.AddScoped<ITelegramBotClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var botConfig = provider.GetRequiredService<IOptions<BotSettings>>().Value;
            if (string.IsNullOrEmpty(botConfig.BotToken))
            {
                throw new InvalidOperationException("The bot token is missing.");
            }

            var options = new TelegramBotClientOptions(
                botConfig.BotToken,
                httpClientFactory.CreateClient(nameof(TelegramBotClient))
            );
            if (!string.IsNullOrWhiteSpace(botConfig.ServerAddress))
            {
                options.ServerAddress = botConfig.ServerAddress;
            }
            return new TelegramBotClient(options);
        });

        services.AddScoped<IStateManager, StateManager>();

        services.RegisterHandlers<IMessageExceptionHandler>();
        services.RegisterHandlers<IUserStateHandler>();
        services.RegisterHandlers<ICommandHandler>();
        services.RegisterHandlers<IMessageHandler>();
        services.AddScoped<UpdateHandler>();

        // Register the update handler pool
        services.AddSingleton<IUpdateHandlerPool, UpdateHandlerPool>();
        services.AddHostedService(provider =>
            (UpdateHandlerPool)provider.GetRequiredService<IUpdateHandlerPool>()
        );

        // Registers other workers
        services.AddHostedService<SetupWorker>();
        services.AddHostedService<LongPollingWorker>();

        return services;
    }

    private static IServiceCollection RegisterHandlers<T>(this IServiceCollection services)
    {
        // Register all command handlers from current assembly
        var commandHandlerTypes = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(T)))
            .OrderBy(t =>
            {
                var attr = t.GetCustomAttribute<PriorityAttribute>();
                return attr is null ? 0 : attr.Priority;
            });
        Console.WriteLine(
            $"Registered {commandHandlerTypes.Count()} {typeof(T).Name} handlers in assembly {Assembly.GetExecutingAssembly().FullName}."
        );
        foreach (var type in commandHandlerTypes)
        {
            services.TryAddScoped(typeof(T), type);
        }

        return services;
    }
}
