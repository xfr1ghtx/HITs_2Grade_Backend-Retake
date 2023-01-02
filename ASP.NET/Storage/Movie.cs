using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Storage;

public class Movie
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string? Poster { get; set; }
    public int Year { get; set; }
    public string? Country { get; set; }
    public int Time { get; set; }
    public string? TagLine { get; set; }
    public string? Description { get; set; }
    public string? Director { get; set; }
    public int? Budget { get; set; }
    public int? Fees { get; set; }
    public int? AgeLimit { get; set; }

    public List<User> FavoriteBy { get; set; } = new();
    public List<Genre> Genres { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}