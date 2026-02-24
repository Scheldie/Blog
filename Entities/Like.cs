using Blog.Entities.Enums;
using Blog.Entities;

namespace Blog.Entities
{
    public class Like : IEntity
    {
        public Like() {}

        public Like(int entityId, int userId, LikeType likeType, User? user)
        {
            EntityId = entityId;
            UserId = userId;
            LikeType = likeType;
            CreatedAt = DateTime.UtcNow;
            User = user;
        }
        public int Id { get; set; }

        public int EntityId { get; init; }

        public virtual User? User { get; init; }

        public int UserId {  get; init; }

        public DateTime CreatedAt { get; init; }

        public LikeType LikeType { get; init; }
    }
}
