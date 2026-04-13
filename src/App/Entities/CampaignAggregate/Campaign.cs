using System.ComponentModel.DataAnnotations;
using App.Configuration;
using App.Entities.Abstractions;

namespace App.Entities.CampaignAggregate;

/// <summary>
/// Represents a campaign
/// </summary>
/// <param name="name">Campaign name</param>
public class Campaign(string name) : TimestampableEntity
{
    /// <summary>
    /// Campaign name
    /// </summary>
    [StringLength(256)]
    public string Name { get; set; } = name;

    /// <summary>
    /// Campaign frequency
    /// </summary>
    public CampaignFrequency Frequency { get; set; }

    /// <summary>
    /// The day of the week when the campaign will be sent
    /// </summary>
    public DayOfWeek Day { get; set; }

    /// <summary>
    /// The time of day when the campaign will be sent
    /// </summary>
    public TimeOnly Time { get; set; }

    /// <summary>
    /// Whether the owners of promoted chats can specify their own schedule
    /// </summary>
    public bool FlexibleSchedule { get; set; }

    /// <summary>
    /// Allowed chat types that can be promoted
    /// </summary>
    public AllowedChatType AllowedChatTypes { get; set; }

    /// <summary>
    /// Maximum number of chats that can be part of a campaign
    /// </summary>
    public int? MaxChats { get; set; }

    /// <summary>
    /// Maximum number of chats that can be added by the same user
    /// </summary>
    public int? MaxChatsPerUser { get; set; }

    /// <summary>
    /// Minimum number of members per promoted chat
    /// </summary>
    public int? MinMembersPerChat { get; set; }

    public required UserEntity Owner { get; set; }

    /// <summary>
    /// The host chat used to manage the campaign
    /// </summary>
    public required ChatEntity Chat { get; set; }

    public ICollection<Template> Templates { get; set; } = [];
}
