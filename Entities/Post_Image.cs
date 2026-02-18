using Blog.Entities;

namespace Blog.Entities
{
    public class Post_Image : IEntity
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public virtual Post? Post { get; set; }

        public int ImageId { get; set; }
        public virtual Image? Image { get; set; }
        public int Order { get; set; }
    }
}
