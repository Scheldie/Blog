using Blog.Data.Interfaces;
using Blog.Entities;

namespace Blog.Data.Repositories
{
    public class PostImageRepository : EntityRepository<Post_Image>, IPostImageRepository
    {

        private readonly BlogDbContext _context;
        public PostImageRepository(BlogDbContext dbContext) : base(dbContext) { }

        public Post_Image GetByImageId(int id)
        {
            return _context.Post_Images.FirstOrDefault(p => p.ImageId == id);
        }
    
    }
}
