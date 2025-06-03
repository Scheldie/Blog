using Blog.Entities.Enums;
using Blog.Entities;

namespace Blog.Entities
{
    public class Like : IEntity
    {
        public int Id { get; set; }

        public int EntityId { get; set; }

        public virtual User User { get; set; }

        public int UserId {  get; set; }

        public DateTime CreatedAt { get; set; }

        public LikeType LikeType { get; set; }
    }
}
