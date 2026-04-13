using App.Configuration;
using App.Data;
using App.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace App.Repositories;

/// <summary>
/// Provides a helper to manage chats data.
/// </summary>
sealed class ChatRepository(AppDbContext context) : RepositoryBase<ChatEntity>(context)
{
    /// <summary>
    /// Retrieves the updated group preferences from the database or creates a entry if it doesn't exist.
    /// If the group already exists, the existing data is updated with the provided information.
    /// </summary>
    /// <param name="message">The message containing the group data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated Telegram group data.</returns>
    public Task<ChatEntity> UpsertFromMessageAsync(
        Message message,
        CancellationToken cancellationToken
    )
    {
        var chatId = message.MigrateFromChatId ?? message.Chat.Id;
        var chatEntity = context
            .Chats.AsNoTrackingWithIdentityResolution()
            .SingleOrDefault(g => g.ChatId == chatId);

        if (chatEntity is null)
        {
            chatEntity = new ChatEntity(message.Chat.Id, message.Chat.Title!)
            {
                Username = message.Chat.Username,
                ChatType =
                    message.Chat.Type == ChatTypes.Channel
                        ? TelegramChatType.Channel
                        : TelegramChatType.Group,
            };
            return this.InsertAsync(chatEntity, cancellationToken);
        }

        if (message.MigrateToChatId is not null)
        {
            chatEntity.ChatId = (long)message.MigrateToChatId;
        }
        chatEntity.Title = message.Chat.Title!;
        chatEntity.Username = message.Chat.Username;
        return this.UpdateAsync(chatEntity, cancellationToken);
    }
}
