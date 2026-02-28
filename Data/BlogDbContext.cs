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
        }
        public BlogDbContext()        
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>()
                .HasOne(u => u.Avatar)
                .WithOne()
                .HasForeignKey<User>(u => u.AvatarId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Blog.Entities.Image>()
                .HasOne(i => i.User)
                .WithMany(u => u.Images)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Post_Image>()
                .HasOne(pi => pi.Image)
                .WithMany(i => i.PostImages)
                .HasForeignKey(pi => pi.ImageId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Post_Image>()
                .HasOne(pi => pi.Post)
                .WithMany(p => p.PostImages)
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .Build();

            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; } 
        public DbSet<Post> Posts { get; set; } 
        public DbSet<Post_Image> Post_Images { get; set; } 
        public DbSet<Comment> Comments { get; set; } 
        public DbSet<Notification> Notifications { get; set; } 
        public DbSet<Post_View> Post_Views { get; set; }
        public DbSet<Follower> Followers { get; set; } 
        public DbSet<Blog.Entities.Image> Images { get; set; } 
        public DbSet<Post_Like> Post_Likes { get; set; } 

        public DbSet<Comment_Like> Comment_Likes { get; set; } 









    }
}
