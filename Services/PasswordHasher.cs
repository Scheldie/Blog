namespace Blog.Services
{
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
