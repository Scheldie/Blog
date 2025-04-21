using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Intefaces;
using Blog.Entities;
using Blog.Models;

namespace Blog.Data.Repositories
{
    public sealed class UserRepository : EntityRepository<User>, IUserRepository
    {
        public UserRepository(BlogDbContext dbContext) : base(dbContext) { }

        private readonly BlogDbContext _context;
        private readonly IMapper _mapper;

        public User GetByEmail(string email)
        {
            return DbContext.Users.FirstOrDefault(user => user.Email == email);
        }
    }
}
