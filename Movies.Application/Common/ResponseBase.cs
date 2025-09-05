namespace Movies.Application.Common;

public class ResponseBase<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }
}

public static class ResponseApiDto
{
    public static ResponseBase<T> Ok<T>(T data, string message = "Success") =>
        new()
        { Success = true, Message = message, Data = data };

    public static ResponseBase<T> Fail<T>(string message) =>
        new()
        { Success = false, Message = message };
}
