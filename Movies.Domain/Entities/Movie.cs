namespace Movies.Domain.Entities
{
   public sealed class Movie
   {
      public int Id { get; set; }
      public string Title { get; set; } = "";
      public int Year { get; set; }
      public string Genres { get; set; } = "";
      public int DurationMinutes { get; set; }
      public string? Synopsis { get; set; }
   }
}
