using System.Security.Claims;
using Blog.Data;
using Blog.Entities;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Services;

public class SearchService(BlogDbContext context)
{
    public async Task<List<UserSearchResultModel>> SearchUsers(int userId, string query, int limit = 10)
    {
        var usersQuery = context.Users
            .Where(u => u.UserName.ToLower().Contains(query.ToLower()) && u.Id != userId)
            .Select(u => new UserSearchResultModel
            {
                Id = u.Id,
                UserName = u.UserName,
                AvatarPath = u.AvatarSmall32Url ?? "",
            })
            .OrderBy(u => u.UserName);

        if (limit > 0) usersQuery = (IOrderedQueryable<UserSearchResultModel>)usersQuery.Take(limit);

        var users = await usersQuery.ToListAsync();
        return users;
    }

    [HttpGet]
    public async Task<List<UserSearchResultModel>> SearchResults(int userId, string query)
    {

        var users = await context.Users
            .Where(u => u.UserName.ToLower().Contains(query.ToLower()) && u.Id != userId)
            .Select(u => new UserSearchResultModel
            {
                Id = u.Id,
                UserName = u.UserName,
                AvatarPath = u.AvatarSmall40Url ?? "",
            })
            .OrderBy(u => u.UserName)
            .ToListAsync();
        
        return users;
    }
}