using Blog.Entities;

namespace Blog.Data.Interfaces
{
    public interface IImageRepository : IEntityRepository<Image>
    {
        public Image GetByPath(string path);

    }
}
