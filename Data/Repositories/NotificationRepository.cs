using Blog.Entities;

namespace Blog.Data.Repositories
{
    public class NotificationRepository : EntityRepository<Notification>
    {

        private readonly BlogDbContext _context;
        public NotificationRepository(BlogDbContext dbContext) : base(dbContext) { }

    }
}
