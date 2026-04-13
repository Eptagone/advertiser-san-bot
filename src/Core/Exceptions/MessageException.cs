using Telegram.BotAPI.AvailableTypes;

namespace App.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when processing a message from a user.
/// </summary>
/// <param name="receivedMessage">The telegram message that triggered the exception</param>
/// <param name="innerException">The inner exception, if any</param>
public abstract class MessageException(Message receivedMessage, Exception? innerException = null)
    : Exception("Failed to process user message", innerException)
{
    /// <summary>
    /// The message sent by the user that triggered the exception
    /// </summary>
    public Message ReceivedMessage { get; set; } = receivedMessage;

    /// <summary>
    /// If the bot already replied to the user, this is the message that was sent
    /// </summary>
    public Message? SentMessage { get; set; }
}
