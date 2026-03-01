using Blog.Entities;

namespace Blog.Entities
{
    public class Follower : IEntity
    {
        public int Id { get; set; }

        public virtual User? FollowerUser { get; set; }
        
        public int FollowerUserId { get; init; }
         
        public virtual User? Following { get; init; }
        public int FollowingId { get; init; }

        public DateTime CreatedAt { get; init; }

    }
}
