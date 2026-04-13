using System.ComponentModel.DataAnnotations;
using App.Entities.Abstractions;

namespace App.Entities;

/// <summary>
/// Represents a user on Telegram.
/// </summary>
public class UserEntity(long userId, string firstName) : LocalizableEntity
{
    /// <summary>
    /// Unique identifier for this user.
    /// </summary>
    public long UserId { get; set; } = userId;

    /// <summary>
    /// User's first name.
    /// </summary>
    [MaxLength(128)]
    public string FirstName { get; set; } = firstName;

    /// <summary>
    /// User's last name.
    /// </summary>
    [MaxLength(128)]
    public string? LastName { get; set; }

    /// <summary>
    /// User's username.
    /// </summary>
    [MaxLength(32)]
    public string? Username { get; set; }

    /// <summary>
    /// Indicates if the user specified a fixed language code.
    /// If true, the language code won't be updated automatically.
    /// </summary>
    public bool UseFixedLanguage { get; set; }

    /// <summary>
    /// User's specified timezone offset.
    /// </summary>
    public TimeSpan? TimezoneOffset { get; set; }
}
