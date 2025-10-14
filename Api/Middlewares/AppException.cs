namespace Api.Middlewares
{
  public class AppException : Exception
  {
    public int StatusCode { get; set; }

    public AppException(string message, int statusCode = Api.Helpers.HttpStatusCode.BadRequest)
        : base(message)
    {
      StatusCode = statusCode;
    }
  }
}
