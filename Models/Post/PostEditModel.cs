using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Post
{
    public class PostEditModel
    {
        public int PostId { get; set; }
        
        [Required(ErrorMessage = "Заголовок обязателен")]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Описание обязательно")]
        [DataType(DataType.Text)]
        [MaxLength(1200)]
        public string Description { get; set; }
        public bool DeleteExistingImages { get; set; }
        public int DeletedExistingImagesCount { get; set; }
        
        public List<string>? DeletedFilesPaths { get; set; } // Коллекция для хранения путей удаляемых файлов

        [DataType(DataType.Upload)]
        public IEnumerable<IFormFile>? NewImageFiles { get; set; }
    }
}
