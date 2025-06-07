using AutoModeratedForum.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoModeratedForum.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<ModerationRequest> ModerationRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Comment>()
               .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>(c => c.User)
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ModerationRequest>()
                .HasOne(m => m.Comment)
                .WithMany()
                .HasForeignKey(m => m.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ModerationRequest>()
                .HasOne<Microsoft.AspNetCore.Identity.IdentityUser>(m => m.Moderator)
                .WithMany()
                .HasForeignKey(m => m.ModeratorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
