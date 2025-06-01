using Blog.Entities;
using Blog.Models;

namespace Blog.Data.Intefaces
{
    public interface IJwtProvider
    {
        public string GenerateToken(User user);


    }
}
