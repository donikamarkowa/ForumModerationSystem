using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using AutoModeratedForum.Enums;

namespace AutoModeratedForum.Entities
{
    public class ModerationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CommentId { get; set; }

        [ForeignKey(nameof(CommentId))]
        public virtual Comment Comment { get; set; } = null!;
        public string? ModeratorId { get; set; }

        [ForeignKey(nameof(ModeratorId))]
        public virtual IdentityUser? Moderator { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public ModerationDecision Decision { get; set; } = ModerationDecision.Pending;

        public string? Notes { get; set; }
    }
}
