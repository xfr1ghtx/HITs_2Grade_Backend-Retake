using Microsoft.EntityFrameworkCore;
using MovieCatalog.Models;
using MovieCatalog.Storage;
using MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

namespace MovieCatalog.Services;

public interface IUsersService
{
    public Task<User?> GetUserById(string Id);
    public Task Update(string userId, ProfileModel data);
}

public class UsersService : IUsersService
{
    private readonly ApplicationDbContext _db;

    public UsersService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetUserById(string Id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id == Id);
    }

    public async Task Update(string userId, ProfileModel data)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == default)
        {
            throw new NotFoundException("Пользователь не найден");
        }

        user.UserName = data.Nickname;
        user.Email = data.Email;
        user.Avatar = data.AvatarLink;
        user.Name = data.Name;
        user.BirthDate = data.BirthDate;
        user.Gender = data.Gender;

        await _db.SaveChangesAsync();
    }
}