using Microsoft.EntityFrameworkCore;
using MovieCatalog.Models;
using MovieCatalog.Storage;
using MovieCatalog.Utility.ExceptionHandler.CustomExceptions;

namespace MovieCatalog.Services;

public interface IMoviesService
{
    Task<MoviesPagedListModel> GetMoviesOnPage(int page);
    Task<MovieDetailsModel> GetMovieBy(Guid id);
    Task<MovieListModel> GetFavoriteMovies(string userId);

    Task AddFavorite(string userId, Guid id);
    Task DeleteFavorite(string userId, Guid id);

    Task AddReview(ReviewModifyModel data, Guid movieId, string userId);
    Task UpdateReview(ReviewModifyModel data, Guid movieId, Guid reviewId, string userId);
    Task DeleteReview(Guid movieId, Guid reviewId, string userId);
}

public class MoviesService : IMoviesService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public MoviesService(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task DeleteReview(Guid movieId, Guid reviewId, string userId)
    {
        
        
        var movie = await _db.Movies
            .Include(m => m.Reviews)
            .FirstOrDefaultAsync(m => m.Id == movieId);

        if (movie == null)
        {
            throw new NotFoundException("Фильма не существует");
        }

        var review = movie.Reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
        {
            throw new NotFoundException("Такого отзыва не существует");
        }
        
        if (review.Author.Id != userId)
        {
            throw new ForbiddenException("Нет доступа");
        }

        movie.Reviews.Remove(review);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateReview(ReviewModifyModel data, Guid movieId, Guid reviewId, string userId)
    {
        var movie = await _db.Movies
            .Include(m => m.Reviews)
            .ThenInclude(r => r.Author)
            .FirstOrDefaultAsync(m => m.Id == movieId);

        if (movie == null)
        {
            throw new NotFoundException("Фильма не существует");
        }

        var review = movie.Reviews.FirstOrDefault(r => r.Id == reviewId);
        
        if (review == null)
        {
            throw new NotFoundException("Такого отзыва не существует");
        }
        
        if (review.Author.Id != userId)
        {
            throw new ForbiddenException("Нет доступа");
        }
        
        review.ReviewText = data.ReviewText;
        review.Rating = data.Rating;
        review.IsAnonymous = data.isAnonymous;

        await _db.SaveChangesAsync();
    }

    public async Task AddReview(ReviewModifyModel data, Guid movieId, string userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("Пользователя не существует");
        }

        var review = new Review
        {
            Author = user,
            CreateDateTime = DateTime.UtcNow,
            IsAnonymous = data.isAnonymous,
            Rating = data.Rating,
            ReviewText = data.ReviewText
        };

        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == movieId);

        if (movie == null)
        {
            throw new NotFoundException("Фильма не существует");
        }

        movie.Reviews.Add(review);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteFavorite(string userId, Guid id)
    {
        var user = await _db.Users
            .Include(u => u.FavoriteMovies)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("Пользователя не существует");
        }

        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            throw new NotFoundException("Фильма не существует");
        }

        user.FavoriteMovies.Remove(movie);
        await _db.SaveChangesAsync();
    }

    public async Task AddFavorite(string userId, Guid id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NotFoundException("Пользователя не существует");
        }

        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            throw new NotFoundException("Фильма не существует");
        }

        user.FavoriteMovies.Add(movie);
        await _db.SaveChangesAsync();
    }

    public async Task<MovieListModel> GetFavoriteMovies(string userId)
    {
        var userMovies = (await _db.Users
            .Include(u => u.FavoriteMovies)
            .ThenInclude(m => m.Genres)
            .Include(u => u.FavoriteMovies)
            .ThenInclude(m => m.Reviews)
            .FirstOrDefaultAsync(u => u.Id == userId))?.FavoriteMovies;

        if (userMovies == null)
        {
            throw new NotFoundException("У пользователя нет любимых фильмов");
        }

        var movies = userMovies.Select(movie => new MovieElementModel
        {
            Id = movie.Id,
            Name = movie.Name,
            Poster = movie.Poster,
            Country = movie.Country,
            Genres = movie.Genres.Select(genre => new GenreModel
            {
                Id = genre.Id,
                Name = genre.Name
            }).ToList(),
            Reviews = movie.Reviews.Select(review => new ReviewShortModel
            {
                Id = review.Id,
                Rating = review.Rating
            }).ToList()
        }).ToList();

        return new MovieListModel
        {
            movies = movies
        };
    }

    public async Task<MovieDetailsModel> GetMovieBy(Guid id)
    {
        var movie = await _db.Movies
            .Select(movie => new MovieDetailsModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Poster = movie.Poster,
                Year = movie.Year,
                Country = movie.Country,
                Time = movie.Time,
                Tagline = movie.TagLine,
                Description = movie.Description,
                Director = movie.Director,
                Budget = movie.Budget,
                Fees = movie.Fees,
                AgeLimit = movie.AgeLimit,
                Genres = movie.Genres.Select(genre => new GenreModel
                {
                    Id = genre.Id,
                    Name = genre.Name
                }).ToList(),
                Reviews = movie.Reviews.Select(review => new ReviewModel
                {
                    Id = review.Id,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    isAnonymous = review.IsAnonymous,
                    CreateDateTime = review.CreateDateTime,
                    Author = new UserShortModel
                    {
                        UserId = review.Author.Id,
                        nickName = review.Author.UserName,
                        avatar = review.Author.Avatar
                    }
                }).ToList()
            })
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            throw new NotFoundException("Данного фильма не существует");
        }

        return movie;
    }

    public async Task<MoviesPagedListModel> GetMoviesOnPage(int page)
    {
        if (page < 1)
        {
            throw new BadRequestException("Неправильная страница");
        }

        var pageSize = _configuration.GetValue<int>("PageSize");
        var firstMovieIndex = (page - 1) * pageSize + 1;
        var countOfMovies = _db.Movies.Count();
        var currentPage = (firstMovieIndex - 1) / pageSize + 1;
        var maxPage = (countOfMovies + pageSize - 1) / pageSize;
        var countTakeMovies = Math.Min(pageSize, countOfMovies - firstMovieIndex + 1);

        if (page > maxPage)
        {
            throw new BadRequestException("Неправильная страница");
        }

        var movies = await _db.Movies
            .OrderByDescending(movie => movie.Name)
            .Skip(firstMovieIndex - 1)
            .Take(countTakeMovies)
            .Include(movie => movie.Genres)
            .Include(movie => movie.Reviews)
            .Select(movie => new MovieElementModel
            {
                Id = movie.Id,
                Name = movie.Name,
                Poster = movie.Poster,
                Year = movie.Year,
                Country = movie.Country,
                Genres = movie.Genres.Select(genre => new GenreModel
                {
                    Id = genre.Id,
                    Name = genre.Name
                }).ToList(),
                Reviews = movie.Reviews.Select(review => new ReviewShortModel
                {
                    Id = review.Id,
                    Rating = review.Rating
                }).ToList()
            }).ToListAsync();

        var pageInfo = new PageInfoModel
        {
            PageCount = maxPage,
            CurrentPage = currentPage,
            PageSize = pageSize
        };

        var moviesPagedList = new MoviesPagedListModel
        {
            Movies = movies,
            PageInfo = pageInfo
        };

        return moviesPagedList;
    }
}