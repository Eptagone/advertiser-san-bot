using App.Core.Services;
using App.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.Extensions.Commands;

namespace App.Bot.Commands;

[BotCommand("help", "How to use the bot")]
[BotCommandVisibility(BotCommandVisibility.PrivateChats)]
class HelpCommand(ITelegramBotClient client, MessageProvider messageProvider) : ICommandHandler
{
    static string? BotUsername;

    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        if (!string.IsNullOrEmpty(message.From?.LanguageCode))
        {
            messageProvider.ChangeCulture(message.From.LanguageCode);
        }

        BotUsername ??= (await client.GetMeAsync(cancellationToken)).Username;
        await client.SendMessageAsync(
            message.Chat.Id,
            messageProvider["Help"],
            replyMarkup: new InlineKeyboardMarkup(
                new InlineKeyboardBuilder().AppendUrl(
                    messageProvider["AddToGroup"],
                    $"https://t.me/{BotUsername}?startgroup"
                )
            ),
            cancellationToken: cancellationToken
        );
    }
}
