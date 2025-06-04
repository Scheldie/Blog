using Blog.Entities;

namespace Blog.Data.Interfaces
{
    public interface IPostImageRepository : IEntityRepository<Post_Image>
    {
        public Post_Image GetByImageId(int id);
    }
}
