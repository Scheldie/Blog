using Blog.Entites;

namespace Blog.Data.Repositories
{
    public class ImageRepository : EntityRepository<Image>
    {
        
        private readonly BlogDbContext _context;
        public ImageRepository(BlogDbContext dbContext) : base(dbContext) { }

    }
}
