using Blog.Data.Intefaces;
using Blog.Models;

namespace Blog.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; } 

        public string UserName { get; set; } 

        public string PasswordHash { get; set; } 

        public string Email { get; set; } 


    }
}
