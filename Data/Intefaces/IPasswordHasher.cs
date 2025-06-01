namespace Blog.Data.Intefaces
{
    public interface IPasswordHasher
    {
        string Generate(string password);

        bool Verify(string password, string hashedPassword);
    }
}
