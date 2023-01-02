namespace MovieCatalog.Models;

public class MoviesPagedListModel
{
    public List<MovieElementModel> Movies { get; set; }
    public PageInfoModel PageInfo { get; set; }
}