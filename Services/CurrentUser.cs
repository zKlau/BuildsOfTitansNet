using BuildsOfTitansNet.Data;
using BuildsOfTitansNet.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildsOfTitansNet.Services
{
    public interface ICurrentUserService
    {
        Task<User?> GetCurrentUserAsync();
        string? GetCurrentUserId();
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public string? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?.FindFirst("userId")?.Value;
            Console.WriteLine("Current User ID: " + (userId ?? "null"));

            return userId;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return null;

            if (int.TryParse(userId, out var userIdInt))
            {
                return await _dbContext.Users.FindAsync(userIdInt);
            }

            return null;
        }
    }
}