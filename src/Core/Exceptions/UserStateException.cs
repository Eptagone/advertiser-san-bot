using Telegram.BotAPI.AvailableTypes;

namespace App.Core.Exceptions;

/// <summary>
/// Thrown when the bot failed to process a user state
/// </summary>
/// <param name="receivedMessage">The message sent by the user that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public abstract class UserStateException(Message receivedMessage, Exception? innerException = null)
    : MessageException(receivedMessage, innerException) { }
