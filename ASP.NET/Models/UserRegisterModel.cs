using MovieCatalog.Storage;

namespace MovieCatalog.Models;

public class UserRegisterModel
{
    public string Username { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
}