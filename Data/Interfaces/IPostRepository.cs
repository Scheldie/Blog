using Blog.Entities;

namespace Blog.Data.Interfaces
{
    public interface IPostRepository : IEntityRepository<Post>
    {
        public IEnumerable<Like> GetLikes(int id);
        public IEnumerable<Comment> GetComments(int id);
    }
}
