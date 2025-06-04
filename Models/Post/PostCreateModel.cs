using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Post
{
    public class PostCreateModel
    {
        [Required(ErrorMessage = "Заголовок обязателен")]
        [DataType(DataType.Text)]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        [DataType(DataType.Text)]
        [MaxLength(1200)]
        public string Description { get; set; }

        [MinLength(1, ErrorMessage = "Добавьте хотя бы одно изображение")]
        public List<IFormFile> ImageFiles { get; set; }
    }
}
