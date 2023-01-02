using MovieCatalog.Storage;

namespace MovieCatalog.Models;

public class ProfileModel
{
    public string Id { get; set; }
    public string Nickname { get; set; }
    public string Email { get; set; }
    public string AvatarLink { get; set; }
    public string Name { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
}