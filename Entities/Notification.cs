using System.ComponentModel.DataAnnotations;
using Blog.Entities.Enums;
using Blog.Entities;

namespace Blog.Entities
{
    public class Notification : IEntity 
    {
        public int Id { get; set; }

        public virtual User? User { get; init; }

        public int UserId { get; init; }

        public NotificationType NotificationType { get; init; }

        public RelatedType RelatedType { get; init; }

        public int RelatedId { get; init; }

        [MaxLength(600)]
        public string? Message { get; init; }

        public bool IsRead { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}
