using Blog.Entities;
using Blog.Models;

namespace Blog.Data.Intefaces
{
    public interface IUserRepository
    {
        public User GetByEmail(string email);
    }
}
