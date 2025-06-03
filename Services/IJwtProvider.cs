using Blog.Entities;
using Blog.Models;

namespace Blog.Services
{
    public interface IJwtProvider
    {
        public string GenerateToken(User user);


    }
}
