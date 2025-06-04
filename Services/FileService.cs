namespace Blog.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            Console.WriteLine($"WebRootPath: {_env.WebRootPath}");
            Console.WriteLine($"File name: {file?.FileName}");
            Console.WriteLine($"File size: {file?.Length}");
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не может быть пустым");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "posts");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return $"/uploads/posts/{uniqueFileName}";
        }
    }
}
