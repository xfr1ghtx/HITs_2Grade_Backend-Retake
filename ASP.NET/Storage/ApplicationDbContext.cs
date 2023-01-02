using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MovieCatalog.Storage;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Token> LogoutTokens { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "fright",
            NormalizedUserName = "FRIGHT",
            PasswordHash =
                "AQAAAAEAACcQAAAAEKrpApckkV0qqYxMM6fg8A6B8UdHV6bLVFggz/smV1/zevuXQv0WME2ARS7Kzs4xhQ==", //Stepan123.
            Name = "Stepan",
            Email = "Stepan@mail.com",
            BirthDate = DateTime.UtcNow,
            Gender = Gender.Male
        });
    }
}