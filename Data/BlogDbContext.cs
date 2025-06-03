using Microsoft.EntityFrameworkCore;
using Blog.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Data.Common;
using Blog.Entities;
using System.Reflection.Emit;
using Blog.Entities.Enums;
using Blog.Models.Account;

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
        public DbSet<PostImage> PostImages { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<PostView> PostViews { get; set; } = null!;
        public DbSet<Follower> Followers { get; set; } = null!;
        public DbSet<Blog.Entities.Image> Images { get; set; } = null!;

        








    }
}
