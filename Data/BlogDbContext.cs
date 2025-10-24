using Blog.Entities;
using Blog.Entities.Enums;
using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace Blog.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
                : base(options) {
            Database.EnsureCreated();
        }
        public BlogDbContext()        
        {

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

        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Post_Image> Post_Images { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Post_View> Post_Views { get; set; } = null!;
        public DbSet<Follower> Followers { get; set; } = null!;
        public DbSet<Blog.Entities.Image> Images { get; set; } = null!;
        public DbSet<Post_Like> Post_Likes { get; set; } = null!;

        public DbSet<Comment_Like> Comment_Likes { get; set; } = null!;









    }
}
