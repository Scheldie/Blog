using Blog.Entites;

namespace Blog.Data.Repositories
{
    public class CommentRepository : EntityRepository<Comment>
    {
        
        private readonly BlogDbContext _context;
        public CommentRepository(BlogDbContext dbContext) : base(dbContext) { }

    }
}
