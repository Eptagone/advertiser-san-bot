namespace App.Core.Configuration;

/// <summary>
/// Defines the settings for the Telegram bot.
/// </summary>
public sealed record BotSettings
{
    /// <summary>
    /// The bot token provided by BotFather.
    /// </summary>
    public string? BotToken { get; set; }

    /// <summary>
    /// Optional. The URL of the application where the bot is hosted.
    /// Required to enable the app features and configure the webhook.
    /// </summary>
    public string? ApplicationUrl { get; set; }

    /// <summary>
    /// Optional. Alternative url to configure the webhook.
    /// </summary>
    public string? WebhookUrl { get; set; }

    /// <summary>
    /// Optional. The secret token used to configure the webhook. If not set, the bot will use long polling.
    /// </summary>
    public string? SecretToken { get; set; }

    /// <summary>
    /// Optional. A custom endpoint where the bot will be sending requests instead of the default one (https://api.telegram.org).
    /// </summary>
    public string? ServerAddress { get; set; }
}
