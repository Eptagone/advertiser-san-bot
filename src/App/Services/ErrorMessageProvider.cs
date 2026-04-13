using App.Core.Resources;
using App.Core.Services;

namespace App.Services;

/// <summary>
/// Accessor for error messages
/// </summary>
class ErrorMessageProvider(IBetterStringLocalizer<ErrorMessageProvider> localizer)
    : IMessageProvider
{
    public string this[string key] => localizer[key];
    public string this[string key, params object[] args] => localizer[key, args];

    public void ChangeCulture(string languageCode)
    {
        localizer.ChangeCulture(languageCode);
    }
}
