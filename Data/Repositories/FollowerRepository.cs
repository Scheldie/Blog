using Blog.Entities;

namespace Blog.Data.Repositories
{
    public class FollowerRepository : EntityRepository<Follower>
    {

        private readonly BlogDbContext _context;
        public FollowerRepository(BlogDbContext dbContext) : base(dbContext) { }

    }
}
