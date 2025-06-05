using Blog.Entities;

namespace Blog.Entities
{
    public class Comment : IEntity
    {
        public int Id { get; set; }

        public virtual User User { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; }

        public virtual Post Post {  get; set; }
        
        public int PostId { get; set; }

        public int ParentId { get; set; }

        public virtual Comment Parent { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual IEnumerable<Like> Likes { get; set; }

        public virtual IEnumerable<Comment> Comments { get; set; }


    }
}
