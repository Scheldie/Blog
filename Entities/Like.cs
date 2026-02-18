using Blog.Entities.Enums;
using Blog.Entities;

namespace Blog.Entities
{
    public class Like : IEntity
    {
        public int Id { get; set; }

        public int EntityId { get; init; }

        public virtual required User User { get; init; }

        public int UserId {  get; init; }

        public DateTime CreatedAt { get; init; }

        public LikeType LikeType { get; init; }
    }
}
