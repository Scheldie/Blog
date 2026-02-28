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
        [MaxLength(160)] 
        public string? AvatarSmall32Url { get; set; } 
        [MaxLength(160)] 
        public string? AvatarSmall40Url { get; set; } 
        [MaxLength(160)] 
        public string? AvatarProfileUrl { get; set; } 
        [MaxLength(160)] 
        public string? AvatarFullUrl { get; set; } 
        [MaxLength(160)] 
        public string? AvatarOriginalUrl { get; set; }

        public virtual ICollection<Post>? Posts { get; init; }

    }
}
