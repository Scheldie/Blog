using System.Security.Claims;
using Blog.Data;
using Blog.Infrastructure.Extensions;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class FeedController : Controller
{
    private readonly BlogDbContext _context;
    private readonly PostService _postService;

    public FeedController(BlogDbContext context, PostService postService)
    {
        _context = context;
        _postService = postService;
    }

    [Route("/Feed")]
    [HttpGet]
    public async Task<IActionResult> Feed(int page = 1)
    {
        var userId = User.GetUserId();
        int pageSize = 1;
        var posts = await _postService.GetFeedPosts(userId, page, pageSize);

        return View(posts);
    }
    [HttpGet]
    [Route("/Feed/GetFeedPostPage")]
    public async Task<IActionResult> GetFeedPostPage(int page = 1)
    {
        var userId = User.GetUserId();
        int pageSize = 1;
        var posts = await _postService.GetFeedPosts(userId, page, pageSize);

        return PartialView("../Post/_PostPartial", posts);
    }
}