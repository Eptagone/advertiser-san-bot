using Telegram.BotAPI.GettingUpdates;

namespace App.Core.Services;

/// <summary>
/// Defines a method to receive updates to be processed later.
/// </summary>
public interface IUpdateHandlerPool
{
    /// <summary>
    /// Receives updates from the Telegram bot.
    /// </summary>
    /// <param name="update">The update.</param>
    void QueueUpdate(Update update);
}
