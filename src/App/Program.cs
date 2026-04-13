using App.Core.Configuration;
using App.Core.Services;
using App.Data;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterAllServices();

var app = builder.Build();

app.MapGet("/", () => new { status = 200 });
app.MapPost(
    "/",
    (
        [FromHeader(Name = TelegramConstants.XTelegramBotApiSecretToken)] string secretToken,
        [FromBody] Update update,
        IUpdateHandlerPool updatePool,
        IOptions<BotSettings> options
    ) =>
    {
        if (options.Value.SecretToken != secretToken)
        {
            return Results.Unauthorized();
        }

        // Queue the update to be processed.
        updatePool.QueueUpdate(update);
        return Results.Accepted();
    }
);

// Apply database migrations if needed
if (
    app.Environment.IsDevelopment()
    || app.Configuration["APPLY_MIGRATIONS"] == "1"
    || app.Configuration["APPLY_MIGRATIONS"] == "true"
)
{
    var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
