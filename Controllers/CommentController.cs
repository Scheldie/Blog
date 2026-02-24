using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Blog.Infrastructure.Extensions;
using Blog.Models.Request;

namespace Blog.Controllers
{
    [Authorize]
    public class CommentController(CommentService service) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetComments(int postId)
        {
            var userId = User.GetUserId();
            return Ok(await service.LoadComments(
                c => c.PostId == postId && c.ParentId == null,
                true,
                false,
                userId
            ));

        }

        [HttpGet]
        public async Task<IActionResult> GetReplies(int commentId)
        {
            var userId = User.GetUserId();
            return Ok(await service.LoadComments(
                c => c.ParentId == commentId,
                false,
                true,
                userId
            ));

        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateRequest model)
        {
            var userId = User.GetUserId();
            var result = await service.AddComment(model, userId);
            if (result == null) return BadRequest("Comment add failed");

            return Ok(new { success = true, result });
        }


        [HttpPost]
        public async Task<IActionResult> EditComment([FromBody] CommentEditRequest model)
        {
            var userId = User.GetUserId();
            if(await service.EditComment(model, userId)) return Ok(new { success = true });

            return BadRequest("Comment edit failed");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = User.GetUserId();
            if(await service.DeleteComment(commentId, userId)) return Ok(new { success = true });

            return BadRequest("Comment delete failed");
        }
    }
}