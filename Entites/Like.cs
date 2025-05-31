using Blog.Entities;

namespace Blog.Entites
{
    public class Like : IEntity
    {
        public int Id { get; set; }

        public int EntityId { get; set; }

        public virtual User User { get; set; }

        public int UserId {  get; set; }

        public LikeType LikeType { get; set; }
    }
}
