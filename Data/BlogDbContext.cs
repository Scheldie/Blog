using Microsoft.EntityFrameworkCore;
using Blog.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Data.Common;
using Blog.Entities;
using System.Reflection.Emit;
using Blog.Entites;
using Blog.Entites.Enums;

namespace Blog.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
                : base(options) {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .Build();

            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<PostImages> PostImages { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;




    }
}
