﻿using Blog.Entities;
using Blog.Entities;

namespace Blog.Entities
{
    public class Post : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual User Author { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime PublishedAt { get; set; }

        public int ImagesCount { get; set; }
        
        public virtual IEnumerable<Post_Image> Post_Images { get; set; }

        public virtual IEnumerable<Post_View> Post_Views { get; set; }

        public virtual IEnumerable<Comment> Comments { get; set; }

        public virtual IEnumerable<Post_Like> Post_Likes { get; set; }

        public int ViewCount { get; set; }
        public int LikesCount { get; set; }
    }
   

}
