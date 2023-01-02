namespace MovieCatalog.Models;

public class ReviewModel
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string ReviewText { get; set; }
    public bool isAnonymous { get; set; }
    public DateTime CreateDateTime { get; set; }
    public UserShortModel Author { get; set; }
}