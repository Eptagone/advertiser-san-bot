using App.Core.Resources;
using App.Core.Services;

namespace App.Services;

/// <summary>
/// Stores the available bot messages
/// </summary>
class MessageProvider(IBetterStringLocalizer<MessageProvider> localizer) : IMessageProvider
{
    public string this[string key] => localizer[key];
    public string this[string key, params object[] args] => localizer[key, args];

    public void ChangeCulture(string languageCode)
    {
        localizer.ChangeCulture(languageCode);
    }
}
