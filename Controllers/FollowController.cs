using Blog.Data;
using Blog.Infrastructure.Extensions;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

public class FollowController : Controller
{
    private readonly FollowService _followService;
    private readonly BlogDbContext _context;
    public FollowController(FollowService followService, BlogDbContext  context)
    {
        _followService = followService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Follow(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null) return NotFound();
        var currentUserId = User.GetUserId();
        var ok = await _followService.FollowAsync(currentUserId, user.Id);

        if (!ok)
            return StatusCode(500, new { success = false });

        return Ok(new { success = true });
    }
    
    [HttpPost]
    public async Task<IActionResult> Unfollow(string userName)
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null) return NotFound();
        var currentUserId = User.GetUserId();
        var ok = await _followService.UnfollowAsync(currentUserId, user.Id);

        if (!ok)
            return StatusCode(500, new { success = false });

        return Ok(new { success = true });
    }
    
    [HttpGet]
    public async Task<IActionResult> Followers(string userName, int page = 1)
    {
        var currentUserId = User.GetUserId();
        var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
        int pageSize = 6;
        var followers = await _followService.GetFollowersAsync(userName, page, pageSize);

        var models = new List<FollowUserModel>();

        foreach (var u in followers)
        {
            models.Add(new FollowUserModel
            {
                Id = u.Id,
                UserName = u.UserName,
                AvatarPath = u.AvatarProfileUrl ?? "",
                IsFollowing = currentUserId != 0 && await _followService.IsFollowingAsync(currentUserId, u.Id),
                CurrentUsername = currentUser.UserName
            });
        }

        return PartialView("_FollowersPartial", models);
    }


    [HttpGet]
    public async Task<IActionResult> Following(string userName, int page = 1)
    {
        var currentUserId = User.GetUserId();
        var currentUser = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
        int pageSize = 6;
        var following = await _followService.GetFollowingAsync(userName, page, pageSize);

        var models = new List<FollowUserModel>();

        foreach (var u in following)
        {
            models.Add(new FollowUserModel
            {
                Id = u.Id,
                UserName = u.UserName,
                AvatarPath = u.AvatarProfileUrl ?? "",
                IsFollowing = currentUserId != 0 && await _followService.IsFollowingAsync(currentUserId, u.Id),
                CurrentUsername = currentUser.UserName
            });
        }

        return PartialView("_FollowingPartial", models);
    }


}
