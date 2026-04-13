using App.Data;
using App.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI.AvailableTypes;

namespace App.Repositories;

/// <summary>
/// Provides a helper to manage user data.
/// </summary>
sealed class UserRepository(AppDbContext context) : RepositoryBase<UserEntity>(context)
{
    /// <summary>
    /// Retrieves the updated user preferences from the database or creates a profile for the user if it doesn't exist.
    /// If the user already exists, the existing data is updated with the provided information.
    /// </summary>
    /// <param name="user">The Telegram user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated Telegram user data.</returns>
    public Task<UserEntity> UpsertAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = context
            .Users.AsNoTrackingWithIdentityResolution()
            .SingleOrDefault(u => u.UserId == user.Id);
        if (userEntity is null)
        {
            userEntity = new UserEntity(user.Id, user.FirstName)
            {
                Username = user.Username,
                LastName = user.LastName,
                LanguageCode = user.LanguageCode,
            };
            return this.InsertAsync(userEntity, cancellationToken);
        }

        userEntity.Username = user.Username;
        userEntity.FirstName = user.FirstName;
        userEntity.LastName = user.LastName;
        if (!userEntity.UseFixedLanguage)
        {
            userEntity.LanguageCode = user.LanguageCode;
        }
        return this.UpdateAsync(userEntity, cancellationToken);
    }
}
