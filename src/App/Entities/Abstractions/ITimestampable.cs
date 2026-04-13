namespace App.Entities.Abstractions;

/// <summary>
/// Defines properties for entities that have a creation and update date.
/// </summary>
public interface ITimestampable
{
    /// <summary>
    /// Date and time of creation.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Date and time of last update.
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }
}
