using App.Entities.Abstractions;

namespace App.Entities.CampaignAggregate;

/// <summary>
/// Represents a message sent to a promoted chat
/// </summary>
public class PromoMessage : TimestampableEntity
{
    public int MessageId { get; set; }
    public PromotedChat PromotedChat { get; set; } = null!;
}
