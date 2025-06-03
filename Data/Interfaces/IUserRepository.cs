using Blog.Entities;
using Blog.Models;

namespace Blog.Data.Interfaces
{
    public interface IUserRepository : IEntityRepository<User>
    {
        public User GetByEmail(string email);

        public User GetById(int id);

    }
}
