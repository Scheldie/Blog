using System.Security.Claims;
using Blog.Data;
using Blog.Entities;
using Blog.Entities.Enums;
using Blog.Infrastructure.Extensions;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

public class LikeController(LikeService service) : Controller
{

    

    [HttpPost]
    public async Task<IActionResult> ToggleLike([FromBody] LikeModel model)
    {
        var userId = User.GetUserId();
        var (isLiked, likesCount) = await service.ToggleLike(model, userId);
        return Ok(new
        {
            success = true, 
            isLiked, 
            likesCount
        });
    }
}