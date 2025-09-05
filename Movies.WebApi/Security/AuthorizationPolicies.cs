namespace Movies.WebApi.Security;

public static class AuthorizationPolicies
{
    public const string AllowedDomain = "AllowedDomain";
    public const string MoviesRead = "Movies.Read";
    public const string MoviesWrite = "Movies.Write";
    public const string MoviesDelete = "Movies.Delete";
}