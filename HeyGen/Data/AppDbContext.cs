using HeyGen.Models;
using Microsoft.EntityFrameworkCore;

namespace HeyGen.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<VideoRequestEntity> VideoRequests { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Voice> Voices { get; set; }

        public DbSet<TextToSpeechEntity> TextToSpeechRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoRequestEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HeyGenVideoId).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(255);
                entity.Property(e => e.JsonRequest).HasColumnType("ntext");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");
            });
            modelBuilder.Entity<Avatar>(entity =>
            {
                entity.HasKey(e => e.AvatarId);
                entity.Property(e => e.AvatarName).HasMaxLength(255);
                entity.Property(e => e.Gender).HasMaxLength(50);
            });

            modelBuilder.Entity<Voice>(entity =>
            {
                entity.HasKey(e => e.VoiceId);
                entity.Property(e => e.Language).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(50);
            });
            modelBuilder.Entity<TextToSpeechEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TaskId).IsRequired();
                entity.Property(e => e.Text).HasColumnType("nvarchar(max)");
                entity.Property(e => e.VoiceId).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.AudioUrl).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime2");
            });
        }
    }
}