using System.Reflection;
using App.Core.Configuration;
using App.Core.Services;
using Castle.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;
using Telegram.BotAPI.GettingUpdates;

namespace App.Core;

sealed class SetupWorker(IServiceProvider serviceProvider, IOptions<BotSettings> options)
    : BackgroundService
{
    private readonly BotSettings settings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        await SetupCommandsAsync(client, stoppingToken);
        await this.ConfigureUpdateStrategyAsync(client, stoppingToken);
    }

    private static async Task SetupCommandsAsync(
        ITelegramBotClient client,
        CancellationToken stoppingToken
    )
    {
        var definitions = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(ICommandHandler)))
            .Aggregate(
                new List<SetMyCommandsArgs>(),
                (acc, t) =>
                {
                    var attr = t.GetAttribute<BotCommandAttribute>();
                    var command = new BotCommand(attr.Command, attr.Description);
                    var visibility = t.GetAttribute<BotCommandVisibilityAttribute?>()?.Visibility;
                    // Ignore hidden commands
                    if (visibility?.HasFlag(BotCommandVisibility.Hidden) == true)
                    {
                        return acc;
                    }
                    // Get scopes
                    var scopes = new List<BotCommandScope?>();
                    foreach (var v in Enum.GetValues<BotCommandVisibility>())
                    {
                        if (visibility?.HasFlag(v) == true)
                        {
                            BotCommandScope? scope = v switch
                            {
                                BotCommandVisibility.PrivateChats =>
                                    new BotCommandScopeAllPrivateChats(),
                                BotCommandVisibility.Members => new BotCommandScopeAllGroupChats(),
                                BotCommandVisibility.Administrators =>
                                    new BotCommandScopeAllChatAdministrators(),
                                _ => null,
                            };
                            scopes.Add(scope);
                        }
                    }
                    if (scopes.Count == 0)
                    {
                        scopes.Add(null);
                    }

                    var translations = t.GetAttributes<LocalizedBotCommandAttribute>()
                        .Select(a => new
                        {
                            a.LanguageCode,
                            Command = new BotCommand(a.Command ?? attr.Command, a.Description),
                        });

                    // For each scope, define the command and translations
                    foreach (var scope in scopes)
                    {
                        var item = acc.FirstOrDefault(a => a.Scope?.Type == scope?.Type);
                        if (item is null)
                        {
                            item = new SetMyCommandsArgs([command]) { Scope = scope };
                            acc.Add(item);
                        }
                        else
                        {
                            item.Commands = item.Commands.Append(command);
                        }

                        foreach (var tc in translations)
                        {
                            var tItem = acc.FirstOrDefault(a =>
                                a.Scope?.Type == scope?.Type && a.LanguageCode == tc.LanguageCode
                            );
                            if (tItem is null)
                            {
                                tItem = new SetMyCommandsArgs([tc.Command])
                                {
                                    Scope = scope,
                                    LanguageCode = tc.LanguageCode,
                                };
                                acc.Add(tItem);
                            }
                            else
                            {
                                tItem.Commands = tItem.Commands.Append(tc.Command);
                            }
                        }
                    }

                    return acc;
                },
                acc =>
                {
                    // Merge default commands with the rest of the commands
                    foreach (var item in acc.Where(a => a.Scope is not null))
                    {
                        var @default = acc.FirstOrDefault(d =>
                            d.Scope is null && d.LanguageCode == item.LanguageCode
                        );
                        if (@default?.Commands is not null)
                        {
                            item.Commands = item.Commands.Concat(@default.Commands);
                        }
                    }
                    return acc;
                }
            );

        foreach (var definition in definitions)
        {
            await client.DeleteMyCommandsAsync(
                definition.Scope,
                definition.LanguageCode,
                stoppingToken
            );
            await client.SetMyCommandsAsync(
                definition.Commands,
                definition.Scope,
                definition.LanguageCode,
                stoppingToken
            );
        }
    }

    private async Task ConfigureUpdateStrategyAsync(
        ITelegramBotClient client,
        CancellationToken stoppingToken
    )
    {
        // Delete the previous webhook if it is configured.
        await client.DeleteWebhookAsync(cancellationToken: stoppingToken);

        var webhookUrl = this.settings.WebhookUrl ?? this.settings.ApplicationUrl;
        if (!string.IsNullOrEmpty(webhookUrl) && !string.IsNullOrEmpty(this.settings.SecretToken))
        {
            var url = new Uri(new Uri(webhookUrl), "webhook");
            await client.SetWebhookAsync(
                url.AbsoluteUri,
                secretToken: this.settings.SecretToken,
                cancellationToken: stoppingToken
            );
        }
    }
}
