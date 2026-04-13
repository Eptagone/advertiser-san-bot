using App.Configuration;
using App.Entities.Abstractions;

namespace App.Entities.CampaignAggregate;

public class Template(string name, string content) : TimestampableEntity
{
    /// <summary>
    /// Template name
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Template content to include in the message
    /// </summary>
    public string Content { get; set; } = content;

    /// <summary>
    /// Template thumbnail
    /// </summary>
    public string? ThumbnailId { get; set; }

    /// <summary>
    /// Template thumbnail type
    /// </summary>
    public TelegramMediaType? ThumbnailType { get; set; }

    public Campaign Campaign { get; set; } = null!;
}
