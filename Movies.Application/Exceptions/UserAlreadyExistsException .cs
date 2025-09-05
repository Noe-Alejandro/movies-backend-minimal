namespace Movies.Application.Exceptions;

public class UserAlreadyExistsException : ApplicationException
{
    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.") { }
}
