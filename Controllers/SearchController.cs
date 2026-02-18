using System.Security.Claims;
using Blog.Data;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class SearchController : Controller
{
    private readonly BlogDbContext _context;

    public SearchController(BlogDbContext context)
    {
        _context = context;
    }
    [HttpGet]
        public async Task<IActionResult> SearchUsers(string query, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return BadRequest("Слишком короткий запрос");
            }

            int currentUserId;
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out currentUserId);

            // Ищем пользователей, чьи имена содержат запрос (регистронезависимо)
            var usersQuery = _context.Users
                .Where(u => u.UserName.ToLower().Contains(query.ToLower()) && u.Id != currentUserId)
                .Select(u => new UserSearchResultModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    AvatarPath = u.AvatarPath ?? "",
                })
                .OrderBy(u => u.UserName);

            if (limit > 0)
            {
                usersQuery = (IOrderedQueryable<UserSearchResultModel>)usersQuery.Take(limit);
            }

            var users = await usersQuery.ToListAsync();

            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> SearchResults(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return RedirectToAction("Users");
            }

            int currentUserId;
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out currentUserId);

            var users = await _context.Users
                .Where(u => u.UserName.ToLower().Contains(query.ToLower()) && u.Id != currentUserId)
                .Select(u => new UserSearchResultModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    AvatarPath = u.AvatarPath ?? "",
                })
                .OrderBy(u => u.UserName)
                .ToListAsync();


            ViewBag.SearchQuery = query;
            return View(users);
        }
}