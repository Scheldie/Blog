using Blog.Data;
using Blog.Entities;
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
        if (model.RemoveAvatar)
        {
            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
                await _fileService.DeleteFileAsync(oldFilePath);
            }
            user.AvatarPath = null;
        }
        if (model.Avatar != null && model.Avatar.Length > 0)
        {
            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
                await _fileService.DeleteFileAsync(oldFilePath);
            }
            user.AvatarPath = await _fileService.SaveFileAsync(model.Avatar);
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