using System.Text.RegularExpressions;
using App.Core.Exceptions;
using App.Services;
using Castle.Core.Internal;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.Extensions.Commands;
using Telegram.BotAPI.GettingUpdates;

namespace App.Core.Services;

/// <summary>
/// Defines a method to handle incoming bot updates
/// </summary>
sealed partial class UpdateHandler(
    ITelegramBotClient client,
    IEnumerable<IUserStateHandler> userStateHandlers,
    IEnumerable<IMessageHandler> messageHandlers,
    IEnumerable<ICommandHandler> commandHandlers,
    IEnumerable<IMessageExceptionHandler> messageExceptionHandlers,
    ILogger<UpdateHandler> logger
)
{
    private static string? BotUsername;

    [GeneratedRegex(@"""(?<arg>[^""]+)""|(?<arg>\S+)", RegexOptions.Compiled)]
    private static partial Regex GetArgPattern();

    /// <summary>
    /// Handles an incoming update
    /// </summary>
    /// <param name="update">The update</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>The task object representing the asynchronous operation</returns>
    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not null)
            {
                // Try to continue user states
                foreach (var handler in userStateHandlers)
                {
                    if (await handler.TryContinueAsync(update.Message, cancellationToken))
                    {
                        return;
                    }
                }

                // Try to handle the message
                foreach (var handler in messageHandlers)
                {
                    if (await handler.TryHandleAsync(update.Message, cancellationToken))
                    {
                        return;
                    }
                }

                // Try to handle commands in the message
                if (BotCommandParser.TryParse(update.Message, out var result))
                {
                    var (commandName, commandArgs, username) = result;

                    // Get the bot username if not already set
                    if (string.IsNullOrEmpty(BotUsername))
                    {
                        var me = await client.GetMeAsync(cancellationToken);
                        BotUsername = me.Username;
                    }

                    // Only handle commands for the current bot
                    if (string.IsNullOrEmpty(username) || username == BotUsername)
                    {
                        var command = commandHandlers.FirstOrDefault(c =>
                        {
                            var type = c.GetType();
                            var aliases = type.GetAttributes<BotCommandAttribute>()
                                .SelectMany(a => a.Aliases.Concat([a.Command]))
                                .Concat(
                                    type.GetAttributes<LocalizedBotCommandAttribute>()
                                        .Select(a => a.Command)
                                )
                                .Where(a => !string.IsNullOrEmpty(a))
                                .Distinct();
                            return aliases.Any(a => a == commandName);
                        });
                        if (command is not null)
                        {
                            IEnumerable<string> args = string.IsNullOrEmpty(commandArgs)
                                ? []
                                : GetArgPattern()
                                    .Matches(commandArgs)
                                    .Select(match => match.Groups["arg"].Value);

                            try
                            {
                                await command.InvokeAsync(
                                    update.Message,
                                    [.. args],
                                    cancellationToken
                                );
                            }
                            catch (MessageException)
                            {
                                throw;
                            }
                            catch (Exception exp)
                            {
                                logger.LogFailedToProcessCommand(commandName, exp);
                                throw new CommandException(commandName, update.Message, exp);
                            }
                        }
                    }
                }
            }
        }
        catch (MessageException e)
        {
            foreach (var handler in messageExceptionHandlers)
            {
                if (await handler.TryHandleAsync(e, cancellationToken))
                {
                    return;
                }
            }

            // This should never happen
            throw;
        }
        catch (Exception)
        {
            // Handle other exceptions
            throw;
        }
    }
}
