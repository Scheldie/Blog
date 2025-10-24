using Blog.Data;
using Blog.Entities;
using Blog.Models.Request;
using Blog.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog.Services
{
    public class ProfileService
    {
        private readonly BlogDbContext context;
        private readonly ImageService imageService;
        public ProfileService(BlogDbContext context, ImageService imageService) { 
            this.context = context;
            this.imageService = imageService;
        }

        public async Task<ProfileResponse> EditProfile(ProfileRequest model, int userId)
        {
            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.UserName = model.UserName;
            user.Bio = model.Bio;

            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                user.AvatarPath = imageService.UploadAvatar(model.Avatar).Result;  
            }
            context.Update(user);
            await context.SaveChangesAsync();

            return new ProfileResponse();
        }
    }
}
