using System.ComponentModel.DataAnnotations;
using App.Configuration;
using App.Entities.Abstractions;

namespace App.Entities;

/// <summary>
/// Represents a telegram group or channel.
/// </summary>
/// <param name="chatId">Unique identifier for this chat.</param>
/// <param name="title">Title of the group or channel.</param>
public class ChatEntity(long chatId, string title) : LocalizableEntity
{
    /// <summary>
    /// Unique identifier for this chat.
    /// </summary>
    public long ChatId { get; set; } = chatId;

    /// <summary>
    /// Title of the group or channel.
    /// </summary>
    [StringLength(256)]
    public string Title { get; set; } = title;

    /// <summary>
    /// Optional. Description of the group or channel.
    /// </summary>
    [StringLength(256)]
    public string? Description { get; set; }

    /// <summary>
    /// Optional. Username of the group or channel.
    /// </summary>
    [StringLength(32)]
    public string? Username { get; set; }

    /// <summary>
    /// Optional. Invite link of the group or channel.
    /// </summary>
    [StringLength(512)]
    public string? InviteLink { get; set; }

    /// <summary>
    /// Optional. Type of the group or channel.
    /// </summary>
    public TelegramChatType ChatType { get; set; }
}
