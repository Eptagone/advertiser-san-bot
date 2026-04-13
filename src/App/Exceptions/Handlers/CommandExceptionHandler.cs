using App.Core.Exceptions;
using App.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.UpdatingMessages;

namespace App.Exceptions.Handlers;

sealed class CommandExceptionHandler(ITelegramBotClient client, ErrorMessageProvider errors)
    : IMessageExceptionHandler
{
    public async Task<bool> TryHandleAsync(
        MessageException exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is CommandException ce)
        {
            await this.HandleAsync(ce, cancellationToken);
            return true;
        }

        return false;
    }

    private async Task HandleAsync(CommandException exception, CancellationToken cancellationToken)
    {
        if (exception.SentMessage is null)
        {
            await client.SendMessageAsync(
                exception.ReceivedMessage.Chat.Id,
                errors["CommandError"],
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await client.EditMessageTextAsync(
                exception.ReceivedMessage.Chat.Id,
                exception.SentMessage.MessageId,
                errors["CommandError"],
                cancellationToken: cancellationToken
            );
        }
    }
}
