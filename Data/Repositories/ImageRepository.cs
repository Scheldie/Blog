using Blog.Data.Interfaces;
using Blog.Entities;

namespace Blog.Data.Repositories
{
    public class ImageRepository : EntityRepository<Image>, IImageRepository
    {
        
        private readonly BlogDbContext _context;
        public ImageRepository(BlogDbContext dbContext) : base(dbContext) { }

        public Image? GetByPath(string path)
        {
            return DbContext.Images.FirstOrDefault(img => img.Path == path);
        }

    }
}
