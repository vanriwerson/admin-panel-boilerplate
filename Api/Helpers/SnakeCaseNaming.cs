namespace Api.Helpers
{
  public static class SnakeCaseNaming
  {
    /// <summary>
    /// Converte PascalCase para snake_case
    /// </summary>
    public static string ToSnakeCase(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
        return input;

      var buffer = new System.Text.StringBuilder();
      for (int i = 0; i < input.Length; i++)
      {
        var c = input[i];
        if (char.IsUpper(c))
        {
          if (i > 0)
            buffer.Append('_');
          buffer.Append(char.ToLower(c));
        }
        else
        {
          buffer.Append(c);
        }
      }
      return buffer.ToString();
    }
  }
}
