using App.Entities;
using App.Repositories;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace App.Services;

// TODO: Not ready
sealed class ContextProvider(UserRepository users, ChatRepository groups)
{
    /// <summary>
    /// Load the user context from the given message.
    /// </summary>
    /// <param name="message">The received message</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns>A tuple containing the user, group, and language code</returns>
    public async Task<UserEntity> LoadAsync(Message message, CancellationToken cancellationToken)
    {
        var (user, _) = await this.LoadAllAsync(message, cancellationToken);
        return user;
    }

    /// <summary>
    /// Load the user context from the given telegram user object
    /// </summary>
    /// <param name="user">The telegram user object</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    public async Task<UserEntity> LoadAsync(User from, CancellationToken cancellationToken)
    {
        var user = await users.UpsertAsync(from, cancellationToken);
        return user;
    }

    /// <summary>
    /// Load and return the full context from the given message.
    /// </summary>
    /// <param name="message">The received message</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation</param>
    /// <returns></returns>
    public async Task<(UserEntity user, ChatEntity? group)> LoadAllAsync(
        Message message,
        CancellationToken cancellationToken
    )
    {
        var isPrivateChat = message.Chat.Type == ChatTypes.Private;
        var user = await users.UpsertAsync(message.From!, cancellationToken);
        var group = isPrivateChat
            ? null
            : await groups.UpsertFromMessageAsync(message, cancellationToken);
        return (user, group);
    }
}
