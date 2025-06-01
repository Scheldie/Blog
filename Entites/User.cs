using Blog.Entites;
using Blog.Models;

namespace Blog.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; } 

        public string UserName { get; set; } 

        public string PasswordHash { get; set; } 

        public string Email { get; set; } 

        public string? Bio { get; set; } //о себе

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public DateTime LastLoginAt { get; set; }

        public virtual Image Avatar { get; set; } 

        public string? AvatarPath { get; set; }

        public virtual IEnumerable<Post> Posts { get; set; }
        public virtual IEnumerable<Comment> Comments { get; set; }

    }
}
