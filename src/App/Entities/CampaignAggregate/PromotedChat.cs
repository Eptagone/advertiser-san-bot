using App.Entities.Abstractions;

namespace App.Entities.CampaignAggregate;

public class PromotedChat : EntityBase
{
    public required ChatEntity Chat { get; set; }
    public required Template Template { get; set; }
    public UserEntity AddedByUser { get; set; } = null!;

    /// <summary>
    /// List tag
    /// </summary>
    public string? ListTag { get; set; }

    /// <summary>
    /// The day of the week when the campaign will be sent
    /// </summary>
    public DayOfWeek? Day { get; set; }

    /// <summary>
    /// The time of day when the campaign will be sent
    /// </summary>
    public TimeOnly? Time { get; set; }

    public ICollection<PromoMessage> Messages { get; set; } = [];
}
