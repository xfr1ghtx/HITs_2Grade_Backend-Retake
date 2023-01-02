using Microsoft.AspNetCore.Identity;

namespace MovieCatalog.Storage;

public class User : IdentityUser
{
    public string Name { get; set; }
    public string? Avatar { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }

    public List<Movie> FavoriteMovies { get; set; } = new();
}