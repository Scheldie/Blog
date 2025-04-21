using Blog.Data.Intefaces;
using Blog.Models;

namespace Blog.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; } 
        public string UserName { get; set; } 
        public string PasswordHash { get; set; } 

        public string Salt { get; set; }
        public string Email { get; set; } 


        public User(int id, string userName, string passwordHash, string salt, string email)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
            Salt = salt;
            Email = email;
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}
