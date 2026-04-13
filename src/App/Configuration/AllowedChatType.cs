namespace App.Configuration;

/// <summary>
/// Represents a Telegram chat type
/// </summary>
[Flags]
public enum AllowedChatType
{
    Groups = 1,
    Channels = 2,
    GroupsAndChannels = Groups | Channels,
}
