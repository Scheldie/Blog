using Blog.Data;
using Blog.Entities;
using Blog.Infrastructure.Images;
using Blog.Models;
using Blog.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class ProfileService
{
    private readonly BlogDbContext _context;
    private readonly IWebHostEnvironment _env;
    private IFileService _fileService;

    public ProfileService(BlogDbContext context, IWebHostEnvironment env, IFileService fileService)
    {
        _context = context;
        _env = env;
        _fileService = fileService;
    }

    public async ValueTask<bool> EditProfile(int userId, ProfileEditModel model)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;
        user.UserName = model.UserName;
        user.Bio = model.Bio;
        var currentAvatar = user.AvatarProfileUrl;
        if (model.RemoveAvatar)
        {
            if (!string.IsNullOrEmpty(currentAvatar))
            {
                await _fileService.DeleteAvatarAsync(userId);

                user.AvatarSmall32Url = null;
                user.AvatarSmall40Url = null;
                user.AvatarProfileUrl = null;
                user.AvatarFullUrl = null;
                user.AvatarOriginalUrl = null;

            }
        }
        if (model.Avatar != null && model.Avatar.Length > 0)
        {
            if (!string.IsNullOrEmpty(user.AvatarProfileUrl))
            {
                await _fileService.DeleteAvatarAsync(userId);

                user.AvatarSmall32Url = null;
                user.AvatarSmall40Url = null;
                user.AvatarProfileUrl = null;
                user.AvatarFullUrl = null;
                user.AvatarOriginalUrl = null;
            }
            var variants = await _fileService.UploadAvatarAsync(model.Avatar, userId);
            user.AvatarSmall32Url = variants[ImageVariant.AvatarSmall32]; 
            user.AvatarSmall40Url = variants[ImageVariant.AvatarSmall40]; 
            user.AvatarProfileUrl = variants[ImageVariant.AvatarProfile]; 
            user.AvatarFullUrl = variants[ImageVariant.AvatarFull]; 
            user.AvatarOriginalUrl = variants[ImageVariant.Original];
        }
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<ProfileModel?> GetUser(int currentUserId, string? name)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.Id == currentUserId);
        User user;
        if (name == null) user = currentUser;
        else user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == name) ?? currentUser;
        var isCurrentUser = user?.Id == currentUserId;
        currentUser.IsActive = true;
        currentUser.LastActiveAt = DateTime.UtcNow;
        if (user == null) return null;
        await _context.SaveChangesAsync();
        return new ProfileModel(user, isCurrentUser);
    }
    public async ValueTask<bool> Logout(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;
        user.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
        
    }
}