using Blog.Data.Intefaces;
using Blog.Entities;

namespace Blog.Entites
{
    public class Like : IEntity
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public User User { get; set; }
    }
}
