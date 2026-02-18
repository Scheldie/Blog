using Blog.Entities;

namespace Blog.Entities
{
    public class Follower : IEntity
    {
        public int Id { get; set; }

        public virtual User? User { get; set; }
        
        public int UserId { get; init; }
         
        public virtual Follower? Follow { get; init; }
        public int FollowerId { get; init; }

        public DateTime CreatedAt { get; init; }

    }
}
