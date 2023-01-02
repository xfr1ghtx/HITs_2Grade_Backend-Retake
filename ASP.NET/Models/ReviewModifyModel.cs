namespace MovieCatalog.Models;

public class ReviewModifyModel
{
    public string ReviewText { get; set; }
    public int Rating { get; set; }
    public bool isAnonymous { get; set; }
}