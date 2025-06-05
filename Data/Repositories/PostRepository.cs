using Blog.Data.Interfaces;
using Blog.Entities;

namespace Blog.Data.Repositories
{
    public class PostRepository : EntityRepository<Post>, IPostRepository
    {
        
        private readonly BlogDbContext _context;
        public PostRepository(BlogDbContext dbContext) : base(dbContext) { }

        public IEnumerable<Like> GetLikes(int id)
        {
            return DbContext.Likes.Where(l => l.EntityId == id && l.LikeType == Entities.Enums.LikeType.Post);
        }
        public IEnumerable<Comment> GetComments(int id)
        {
            return DbContext.Comments.Where(c => c.PostId == id);
        }

    }
}
