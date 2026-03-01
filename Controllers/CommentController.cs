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
        public async Task<IActionResult> GetComments(int postId, int page = 1)
        {
            var userId = User.GetUserId();
            var pageSize = 3;
            var comments = await service.LoadCommentsPaged(
                c => c.PostId == postId && c.ParentId == null,
                false,
                userId,
                page,
                pageSize
            );
            return PartialView("_CommentPartialList", comments);

        }

        [HttpGet]
        public async Task<IActionResult> GetReplies(int commentId, int page = 1)
        {
            var userId = User.GetUserId();
            var pageSize = 3;
            var replies = await service.LoadCommentsPaged(
                c => c.ParentId == commentId,
                true,
                userId,
                page,
                pageSize
            );
            return PartialView("_CommentPartialList", replies);

        }

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateModel model)
        {
            var userId = User.GetUserId();
            var result = await service.AddComment(model, userId);
            if (result == null) return BadRequest("Comment add failed");

            return PartialView("_CommentPartial", result);
        }


        [HttpPost]
        public async Task<IActionResult> EditComment([FromBody] CommentEditModel model)
        {
            var userId = User.GetUserId();
            var comment = await service.EditComment(model, userId);

            return PartialView("_CommentPartial", comment);
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