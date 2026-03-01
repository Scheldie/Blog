using Blog.Data;
using Blog.Entities;
using Microsoft.EntityFrameworkCore;

public class FollowService
{
    private readonly BlogDbContext _context;

    public FollowService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<bool> FollowAsync(int followerId, int followingId)
    {
        if (followerId == followingId) return false;

        bool exists = await _context.Followers
            .AnyAsync(f => f.FollowerUserId == followerId && f.FollowingId == followingId);

        if (exists) return false;

        _context.Followers.Add(new Follower
        {
            FollowerUserId = followerId,
            FollowingId = followingId,
            CreatedAt = DateTime.UtcNow
        });
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == followerId);
        var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == followingId);
        currentUser.FollowingCount++;
        targetUser.FollowersCount++;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnfollowAsync(int followerId, int followingId)
    {
        var follow = await _context.Followers
            .FirstOrDefaultAsync(f => f.FollowerUserId == followerId && f.FollowingId == followingId);

        if (follow == null) return false;

        _context.Followers.Remove(follow);
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == followerId);
        var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == followingId);
        currentUser.FollowingCount--;
        targetUser.FollowersCount--;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> IsFollowingAsync(int currentUserId, int targetUserId)
    {
        return await _context.Followers
            .AnyAsync(f => f.FollowerUserId == currentUserId && f.FollowingId == targetUserId);
    }


    public async Task<List<User?>> GetFollowersAsync(string userName, int page, int pageSize)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.UserName == userName);
        return await _context.Followers
            .Where(f => user != null && f.FollowingId == user.Id)
            .OrderByDescending(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => f.FollowerUser)
            .ToListAsync();
    }

    public async Task<List<User?>> GetFollowingAsync(string userName, int page, int  pageSize)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u=>u.UserName == userName);
        return await _context.Followers
            .Where(f => user != null && f.FollowerUserId == user.Id)
            .OrderByDescending(f => f.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => f.Following)
            .ToListAsync();
    }

}