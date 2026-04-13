using App.Core.Exceptions;
using App.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace App.Exceptions.Handlers;

/// <summary>
/// Handles user state exceptions
/// </summary>
sealed class UserStateExceptionHandler(ITelegramBotClient client, ErrorMessageProvider errors)
    : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is UserStateException use)
        {
            await this.HandleAsync(use, cancellationToken);
            return true;
        }

        return false;
    }

    public async Task HandleAsync(UserStateException exception, CancellationToken cancellationToken)
    {
        if (exception is UserStateCancelledException)
        {
            await client.SendMessageAsync(
                exception.ReceivedMessage.Chat.Id,
                errors["UserStateCancelled"],
                replyParameters: new()
                {
                    AllowSendingWithoutReply = true,
                    MessageId = exception.ReceivedMessage.MessageId,
                },
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await client.SendMessageAsync(
                exception.ReceivedMessage.Chat.Id,
                errors["CommandsDisabled"],
                parseMode: FormatStyles.HTML,
                replyParameters: new()
                {
                    AllowSendingWithoutReply = true,
                    MessageId = exception.ReceivedMessage.MessageId,
                },
                cancellationToken: cancellationToken
            );
        }
    }
}
