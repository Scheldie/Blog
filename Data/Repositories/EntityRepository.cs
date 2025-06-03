using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Repositories
{
    public abstract class EntityRepository<IEntity> where IEntity : class, Entities.IEntity
    {
        private protected readonly BlogDbContext DbContext;

        protected EntityRepository(BlogDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IEntity GetEntity(int entityId)
        {
            return DbContext.Find<IEntity>(entityId);
        }

        public void AddEntity(IEntity entity)
        {
            DbContext.Add(entity);
        }

        public void DeleteEntity(int entityId)
        {
            var entity = DbContext.Find<IEntity>(entityId);
            DbContext.Remove(entity);
        }
        public void UpdateEntity(IEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Save()
        {
            DbContext.SaveChanges();
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
