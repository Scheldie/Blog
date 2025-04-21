using Blog.Data.Intefaces;
using Blog.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Blog.Entities
{
    public class Post : IEntity
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public User Author { get; set; }

        public DateTime Date { get; set; }

        public Post(int id, string title, string description, User author, DateTime date)
        {
            Id = id; 
            Title = title; 
            Description = description;  
            Author = author; 
            Date = date;
        }
    }
}
