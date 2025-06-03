using Blog.Entities;

namespace Blog.Entities
{
    public class Follower : IEntity
    {
        public int Id { get; set; }

        public virtual User User { get; set; }
        
        public int UserId { get; set; }
         
        public virtual Follower Follow { get; set; }
        public int FollowerId { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
