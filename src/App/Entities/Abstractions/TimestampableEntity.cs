namespace App.Entities.Abstractions;

/// <summary>
/// Defines a base class for entities that have a creation and update date.
/// </summary>
public abstract class TimestampableEntity : EntityBase, ITimestampable
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
