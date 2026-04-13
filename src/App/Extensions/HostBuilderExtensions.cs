using System.Text.Json;
using App.Core.DI;
using App.Data;
using App.Services;
using Microsoft.EntityFrameworkCore;

namespace App.Extensions;

static class HostBuilderExtensions
{
    public static IHostApplicationBuilder RegisterAllServices(this IHostApplicationBuilder builder)
    {
        // Add caching services.
        var cacheConnectionString = builder.Configuration.GetConnectionString("Cache");
        if (string.IsNullOrEmpty(cacheConnectionString))
        {
            builder.Services.AddDistributedMemoryCache();
        }
        else
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheConnectionString;
                options.InstanceName = "advertiser_san/";
            });
        }

        // Add localization
        builder.Services.AddBetterLocalization();
        builder.Services.AddScoped<MessageProvider>();
        builder.Services.AddScoped<ErrorMessageProvider>();

        // Configure the database
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSnakeCaseNamingConvention()
        );

        // Add bot services
        builder.Services.AddTelegramBot(options =>
        {
            options.BotToken = builder.Configuration["BOT_TOKEN"];
            options.ApplicationUrl = builder.Configuration["APPLICATION_URL"];
            options.WebhookUrl = builder.Configuration["WEBHOOK_URL"];
            options.SecretToken = builder.Configuration["SECRET_WEBHOOK_TOKEN"];
            options.ServerAddress = builder.Configuration["TELEGRAM_BOT_SERVER_ADDRESS"];
            Console.WriteLine($"Bot Token: {options.BotToken}");
        });

        return builder;
    }
}
