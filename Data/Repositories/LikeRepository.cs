using Blog.Entites;

namespace Blog.Data.Repositories
{
    public class LikeRepository : EntityRepository<Like>
    {
        
        private readonly BlogDbContext _context;
        public LikeRepository(BlogDbContext dbContext) : base(dbContext) { }

    }
}
