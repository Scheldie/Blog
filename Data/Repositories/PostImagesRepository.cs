using Blog.Entites;

namespace Blog.Data.Repositories
{
    public class PostImagesRepository : EntityRepository<PostImages>
    {

        private readonly BlogDbContext _context;
        public PostImagesRepository(BlogDbContext dbContext) : base(dbContext) { }
    
    }
}
