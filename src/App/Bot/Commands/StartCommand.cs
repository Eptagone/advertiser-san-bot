using App.Core.Services;
using App.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.Extensions.Commands;

namespace App.Bot.Commands;

[BotCommand("start", "Start the bot")]
[BotCommandVisibility(BotCommandVisibility.Hidden)]
class StartCommand(ITelegramBotClient client, MessageProvider messageProvider) : ICommandHandler
{
    static string? BotUsername;

    public async Task InvokeAsync(
        Message message,
        string[] args,
        CancellationToken cancellationToken
    )
    {
        await client.SendChatActionAsync(
            message.Chat.Id,
            ChatActions.Typing,
            cancellationToken: cancellationToken
        );
        if (!string.IsNullOrEmpty(message.From?.LanguageCode))
        {
            messageProvider.ChangeCulture(message.From.LanguageCode);
        }

        var startParam = args.FirstOrDefault();
        BotUsername ??= (await client.GetMeAsync(cancellationToken)).Username;

        switch (startParam)
        {
            default:
                await client.SendMessageAsync(
                    message.Chat.Id,
                    messageProvider["StartEmpty"],
                    replyMarkup: new InlineKeyboardMarkup(
                        new InlineKeyboardBuilder().AppendUrl(
                            messageProvider["AddToGroup"],
                            $"https://t.me/{BotUsername}?startgroup"
                        )
                    ),
                    cancellationToken: cancellationToken
                );
                break;
        }
    }
}
