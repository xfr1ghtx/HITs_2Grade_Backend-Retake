using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Storage;

public class Token
{
    [Key]
    public string Value { get; set; }

    public DateTime ExpiredDate { get; set; }
}