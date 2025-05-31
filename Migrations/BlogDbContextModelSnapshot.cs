using Blog.Data;
using Blog.Entites.Enums;
using Blog.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Blog.Migrations
{
    [DbContext(typeof(BlogDbContext))]
    public class BlogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // Users 
            modelBuilder.Entity("Blog.Entities.User", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<int>("ImageId")
                    .HasColumnType("integer");

                b.Property<string>("Bio")
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAt")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("UserName")
                    .HasColumnType("text");

                b.Property<DateTime?>("LastLoginAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("UpdatedAt")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.HasIndex("UserName")
                    .IsUnique();

                b.ToTable("Users");
            });

            // Posts 
            modelBuilder.Entity("Blog.Entities.Post", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAt")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");


                b.Property<DateTime?>("PublishedAt")
                    .HasColumnType("timestamp with time zone");


                b.Property<DateTime>("UpdatedAt")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.Property<int>("UserId")
                    .HasColumnType("integer");

                b.Property<int>("ViewCount")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasDefaultValue(0);
                b.Property<int>("LikesCount")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("integer")
                    .HasDefaultValue(0);

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("Posts");
            });

            // Comments
            modelBuilder.Entity("Blog.Entities.Comment", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("Text")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAt")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.Property<DateTime>("UpdatedAt")
                    .ValueGeneratedOnAddOrUpdate()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.Property<int?>("ParentId")
                    .HasColumnType("integer");

                b.Property<int>("PostId")
                    .HasColumnType("integer");

                b.Property<int?>("UserId")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("ParentId");

                b.HasIndex("PostId");

                b.HasIndex("UserId");

                b.ToTable("Comments");
            });

            // Images
            modelBuilder.Entity("Blog.Entities.Image", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("Name")
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAt")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.Property<string>("Path")
                    .IsRequired()
                    .HasColumnType("text");


                b.Property<int>("UserId")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("Images");
            });

            // Post_Images
            modelBuilder.Entity("Blog.Entities.PostImages", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<int>("ImageId")
                    .HasColumnType("integer");

                b.Property<int>("Order")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasDefaultValue(0);

                b.Property<int>("PostId")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("ImageId");

                b.HasIndex("PostId", "ImageId")
                    .IsUnique();

                b.ToTable("Post_Images");
            });


            // Likes
            modelBuilder.Entity("Blog.Entities.Like", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<DateTime>("CreatedAt")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.Property<int>("EntityId")
                    .HasColumnType("integer");

                b.Property<int>("LikeType")
                    .IsRequired();
                    
                b.Property<int>("UserId")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("UserId", "EntityId", "LikeType")
                    .IsUnique();

                b.ToTable("Likes");
            });

            // Followers
            modelBuilder.Entity("Blog.Entities.Follower", b =>
            {
                b.Property<int>("FollowerId")
                    .HasColumnType("integer");

                b.Property<int>("UserId")
                    .HasColumnType("integer");

                b.Property<DateTime>("CreatedAt")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.HasKey("FollowerId", "UserId");

                b.HasIndex("UserId");

                b.ToTable("Followers");
            });


            // Notifications Table
            modelBuilder.Entity("Blog.Entities.Notification", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<DateTime>("CreatedAt")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()");

                b.Property<bool>("IsRead")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("boolean")
                    .HasDefaultValue(false);

                b.Property<string>("Message")
                    .HasColumnType("text");

                b.Property<int>("RelatedId")
                    .HasColumnType("integer");

                b.Property<int>("RelatedType")
                    .IsRequired()
                    .HasColumnType("integer");

                b.Property<int>("NotificationType")
                    .IsRequired()
                    .HasColumnType("integer");

                b.Property<int>("UserId")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.HasIndex("UserId", "IsRead", "CreatedAt");

                b.ToTable("Notifications");

                b.HasCheckConstraint("check_NotificationType", "NotificationType IN ('like', 'comment', 'follow')");

                b.HasCheckConstraint("check_RelatedType", "RelatedType IN ('post', 'comment', 'user')");
            });

            // Post_Views 
            modelBuilder.Entity("YourProject.Models.PostView", b =>
            {
                b.Property<int>("PostId")
                    .HasColumnType("integer");

                b.Property<DateOnly>("Date")
                    .HasColumnType("date");

                b.Property<int>("Count")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasDefaultValue(1);

                b.HasKey("PostId", "Date");

                b.ToTable("Post_Views");
            });

            // Отношения таблиц
            modelBuilder.Entity("Blog.Entities.Post", b =>
            {
                b.HasOne("Blog.Entities.User", "User")
                    .WithMany("Posts")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

            modelBuilder.Entity("Blog.Entities.Comment", b =>
            {
                b.HasOne("Blog.Entities.Comment", "Parent")
                    .WithMany("Replies")
                    .HasForeignKey("ParentId")
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne("Blog.Entities.Post", "Post")
                    .WithMany("Comments")
                    .HasForeignKey("PostId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Blog.Entities.User", "User")
                    .WithMany("Comments")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.SetNull);

                b.Navigation("Parent");
                b.Navigation("Post");
                b.Navigation("User");
            });

            modelBuilder.Entity("Blog.Entities.Image", b =>
            {
                b.HasOne("Blog.Entities.User", "User")
                    .WithMany("Images")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

            modelBuilder.Entity("Blog.Entities.PostImages", b =>
            {
                b.HasOne("Blog.Entities.Image", "Image")
                    .WithMany("PostImages")
                    .HasForeignKey("ImageId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Blog.Entities.Post", "Post")
                    .WithMany("PostImages")
                    .HasForeignKey("PostId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Image");
                b.Navigation("Post");
            });

            modelBuilder.Entity("Blog.Entities.Like", b =>
            {
                b.HasOne("Blog.Entities.User", "User")
                    .WithMany("Likes")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

            modelBuilder.Entity("Blog.Entities.Follower", b =>
            {
                b.HasOne("Blog.Entities.User", "Follower")
                    .WithMany("User")
                    .HasForeignKey("FollowerId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Blog.Entities.User", "User")
                    .WithMany("Followers")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Follower");
                b.Navigation("User");
            });

            modelBuilder.Entity("Blog.Entities.Notification", b =>
            {
                b.HasOne("Blog.Entities.User", "User")
                    .WithMany("Notifications")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("User");
            });

            modelBuilder.Entity("Blog.Entities.PostView", b =>
            {
                b.HasOne("Blog.Entities.Post", "Post")
                    .WithMany("PostViews")
                    .HasForeignKey("PostId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Post");
            });

            modelBuilder.Entity("Blog.Entities.Comment", b =>
            {
                b.Navigation("Replies");
            });

            modelBuilder.Entity("Blog.Entities.Media", b =>
            {
                b.Navigation("PostImages");
            });

            modelBuilder.Entity("Blog.Entities.Post", b =>
            {
                b.Navigation("Comments");
                b.Navigation("PostImages");
                b.Navigation("PostViews");
            });

            modelBuilder.Entity("YourProject.Models.User", b =>
            {
                b.Navigation("Comments");
                b.Navigation("Followers");
                b.Navigation("Following");
                b.Navigation("Likes");
                b.Navigation("Images");
                b.Navigation("Notifications");
                b.Navigation("Posts");
            });
#pragma warning restore 612, 618
        }
    }
}
