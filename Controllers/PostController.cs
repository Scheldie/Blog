using System.Security.Claims;
using Blog.Infrastructure.Extensions;
using Blog.Models;
using Blog.Models.Request;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;


namespace Blog.Controllers;

public class PostController : Controller
{

    private readonly ILogger<ProfileController> _logger;
    private readonly PostService _postService;

    public PostController(ILogger<ProfileController> logger, PostService postService)
    {
        _logger = logger;
        _postService = postService;
    }
    [HttpGet]
    public async Task<IActionResult> LoadPosts(string userName,int page = 1)
    {
        int pageSize = 1;
        var userId = User.GetUserId();
        var posts = await _postService.GetPostsPageAsync(userId,userName, page, pageSize);

        return PartialView("_PostPartial", posts);
    }
    [HttpGet]
    public async Task<IActionResult> GetPost(int id)
    {
        var userId = User.GetUserId();
        var post = await _postService.GetPostById(userId, id);
        if (post == new PostModel(){Title = "",Description = ""}) return NotFound();

        return PartialView("_PostPartial", new List<PostModel>{post});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost([FromForm] PostCreateRequest model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.GetUserId();
        var post = await _postService.CreatePost(userId, model);
        if (post == null)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to create post"
            });
        }

        return Ok(new
        {
            success = true,
            postId = post.Id,
            imagesCount = post.ImagesCount
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> EditPost([FromForm] PostEditRequest model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.GetUserId();

        var success = await _postService.EditPost(userId, model);
        if (success) return Ok(new { success = true });
        return StatusCode(500, new
        {
            success = false,
            message = "Failed to edit post"
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var userId = User.GetUserId();

        var success = await _postService.DeletePost(userId, postId);
        if(success) return Ok(new { success = true });
        return StatusCode(500, new
            {
                success = false,
                message = "Failed to delete post"
            }
        );
    }
}