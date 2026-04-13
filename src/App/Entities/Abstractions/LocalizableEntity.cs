using System.ComponentModel.DataAnnotations;

namespace App.Entities.Abstractions;

/// <summary>
/// Defines a base class for entities that define a language code.
/// </summary>
public abstract class LocalizableEntity : TimestampableEntity, ILocalizableEntity
{
    /// <inheritdoc />
    [StringLength(8)]
    public string? LanguageCode { get; set; }
}
