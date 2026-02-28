using System.Security.Claims;
using Blog.Data;
using Blog.Infrastructure.Extensions;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class SearchController(SearchService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> SearchUsers(string query, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return BadRequest("Слишком короткий запрос");
        }
        var userId = User.GetUserId();
        var users = await service.SearchUsers(userId, query, limit);

        return Ok(users);
    }

    [HttpGet]
    public async Task<IActionResult> SearchResults(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            return RedirectToAction("Users");
        }

        var userId = User.GetUserId();

        var  users = await service.SearchResults(userId, query);

        ViewBag.SearchQuery = query;
        return View(users);
    }
}