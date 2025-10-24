namespace Blog.Services
{
    public interface IPasswordHasher
    {
        string Generate(string password);
        bool Verify(string password, string hash);
    }
    public class PasswordHasher : IPasswordHasher
    {
        public string Generate(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
        }

        public bool Verify(string value, string hash) =>
            BCrypt.Net.BCrypt.Verify(value, hash);
 
    }
}
