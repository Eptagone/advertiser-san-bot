using App.Core.Configuration;
using App.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace App.Core;

class LongPollingWorker(IOptions<BotSettings> options, IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly bool useLongPolling =
        string.IsNullOrEmpty(options.Value.WebhookUrl)
        || string.IsNullOrEmpty(options.Value.SecretToken);

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (this.useLongPolling)
        {
            var scope = serviceProvider.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var updatePool = scope.ServiceProvider.GetRequiredService<IUpdateHandlerPool>();

            IEnumerable<Update> updates = [];
            while (!stoppingToken.IsCancellationRequested)
            {
                if (updates.Any())
                {
                    // Pass the updates to the update receiver.
                    foreach (var update in updates)
                    {
                        updatePool.QueueUpdate(update);
                    }

                    // Get offset for the next update.
                    var offset = updates.Max(u => u.UpdateId) + 1;
                    updates = await client.GetUpdatesAsync(
                        offset,
                        cancellationToken: stoppingToken
                    );
                }
                else
                {
                    // Wait 100 ms before polling again.
                    await Task.Delay(100, stoppingToken);
                    // Get updates from the bot API.
                    updates = await client.GetUpdatesAsync(cancellationToken: stoppingToken);
                }
            }
        }
    }
}
