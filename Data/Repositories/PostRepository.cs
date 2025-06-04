using Blog.Data.Interfaces;
using Blog.Entities;

namespace Blog.Data.Repositories
{
    public class PostRepository : EntityRepository<Post>, IPostRepository
    {
        
        private readonly BlogDbContext _context;
        public PostRepository(BlogDbContext dbContext) : base(dbContext) { }

    }
}
