using Microsoft.Extensions.Localization;

namespace App.Core.Services;

public interface IMessageProvider
{
    /// <summary>
    /// Change the culture for the message provider
    /// </summary>
    /// <param name="languageCode">The language code</param>
    /// <returns></returns>
    void ChangeCulture(string languageCode);

    string this[string name] { get; }

    string this[string name, params object[] arguments] { get; }
}
