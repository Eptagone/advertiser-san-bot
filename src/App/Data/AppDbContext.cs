using App.Data.Configuration;
using App.Entities;
using App.Entities.CampaignAggregate;
using Microsoft.EntityFrameworkCore;

namespace App.Data;

partial class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ChatEntity> Chats { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Template> Templates { get; set; }
    public DbSet<PromotedChat> PromotedChats { get; set; }
    public DbSet<PromoMessage> PromoMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().HasIndex(u => u.UserId).IsUnique();
        modelBuilder.Entity<ChatEntity>().HasIndex(u => u.ChatId).IsUnique();

        modelBuilder.ApplyConfiguration(new TimestampableConfiguration<UserEntity>());
        modelBuilder.ApplyConfiguration(new TimestampableConfiguration<ChatEntity>());
        modelBuilder.ApplyConfiguration(new TimestampableConfiguration<Campaign>());
        modelBuilder.ApplyConfiguration(new TimestampableConfiguration<Template>());
        modelBuilder.ApplyConfiguration(new TimestampableConfiguration<PromoMessage>());
    }
}
