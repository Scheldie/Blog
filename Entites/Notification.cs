using Blog.Entites.Enums;
using Blog.Entities;

namespace Blog.Entites
{
    public class Notification : IEntity 
    {
        public int Id { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        public NotificationType NotificationType { get; set; }

        public RelatedType RelatedType { get; set; }

        public int RelatedId { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
