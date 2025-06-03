using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Interfaces;
using Blog.Entities;
using Blog.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Blog.Data.Repositories
{
    public sealed class UserRepository : EntityRepository<User>, IUserRepository
    {
        public UserRepository(BlogDbContext dbContext) : base(dbContext) { }

        public User? GetByEmail(string email)
        {
            return DbContext.Users.FirstOrDefault(user => user.Email == email);
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await DbContext.Users.FindAsync(id);
        }
        public User GetById(int id)
        {
            return DbContext.Users.FirstOrDefault(user => user.Id == id);
        }
    }
}
