using System.ComponentModel.DataAnnotations;
using Blog.Entities;
using Blog.Models;

namespace Blog.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; } 
        
        [StringLength(30)]
        public required string UserName { get; set; } 

        public bool IsActive { get; set; }

        [StringLength(100)]
        public required string PasswordHash { get; init; } 

        [StringLength(50)]
        public required string Email { get; init; } 

        [StringLength(500)]
        public string? Bio { get; set; }

        public DateTime CreatedAt { get; init; }

        public DateTime LastUpdatedAt { get; init; }

        public DateTime LastLoginAt { get; set; }

        public DateTime LastActiveAt { get; set; }

        public int? AvatarId { get; set; }
        public virtual Image? Avatar { get; init; } 

        [StringLength(200)]
        public string? AvatarPath { get; set; }

        public virtual ICollection<Image>? Images { get; set; }
        public virtual IEnumerable<Post>? Posts { get; init; }
        public virtual IEnumerable<Comment>? Comments { get; init; }

    }
}
