using Blog.Data.Interfaces;
using Blog.Entities;

namespace Blog.Data.Repositories
{
    public class PostImageRepository : EntityRepository<PostImage>, IPostImageRepository
    {

        private readonly BlogDbContext _context;
        public PostImageRepository(BlogDbContext dbContext) : base(dbContext) { }

        public PostImage GetByImageId(int id)
        {
            return _context.PostImages.FirstOrDefault(p => p.ImageId == id);
        }
    
    }
}
