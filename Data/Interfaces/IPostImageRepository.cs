using Blog.Entities;

namespace Blog.Data.Interfaces
{
    public interface IPostImageRepository : IEntityRepository<PostImage>
    {
        public PostImage GetByImageId(int id);
    }
}
