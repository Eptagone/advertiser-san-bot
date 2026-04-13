using App.Core.Services;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions.Commands;

namespace App.Bot.Commands;

[BotCommand("start", "Start the bot")]
[BotCommandVisibility(BotCommandVisibility.Hidden)]
public class StartCommand : ICommandHandler
{
    public Task InvokeAsync(Message message, string[] args, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
