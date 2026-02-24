using System.Security.Claims;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class FeedController : Controller
{
    private readonly BlogDbContext _context;


    public FeedController(BlogDbContext context)
    {
        _context = context;
    }
    [Route("/Feed")]
    [HttpGet]
    public async Task<IActionResult> Feed()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) 
            return Unauthorized();
        var user = await _context.Users.FindAsync(userId);
        var posts = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.PostImages)
            .ThenInclude(pi => pi.Image)
            .Include(p => p.PostLikes)
            .ThenInclude(pl => pl.Like)
            .ThenInclude(l => l.User)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var model = posts.Select(post => post.ToModel(userId)).ToList();

        return View(model);
    }
}