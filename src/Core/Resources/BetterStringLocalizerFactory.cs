using System.Reflection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Core.Resources;

sealed class BetterStringLocalizerFactory(
    IOptions<LocalizationOptions> localizationOptions,
    ILoggerFactory loggerFactory
)
    : ResourceManagerStringLocalizerFactory(localizationOptions, loggerFactory),
        IStringLocalizerFactory
{
    private readonly ILoggerFactory loggerFactory = loggerFactory;
    private readonly IResourceNamesCache resourceNamesCache = new ResourceNamesCache();
    private readonly ICollection<IBetterStringLocalizer> createdLocalizes = [];

    protected override ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(
        Assembly assembly,
        string baseName
    )
    {
        var localizer = new BetterStringLocalizer(
            new ResourceManager(baseName, assembly),
            assembly,
            baseName,
            this.resourceNamesCache,
            this.loggerFactory.CreateLogger<BetterStringLocalizer>()
        );

        this.createdLocalizes.Add(localizer);
        return localizer;
    }
}
