using System;
using System.Collections.Generic;

namespace Api.Helpers
{
  public static class EndpointPermissions
  {
    // Chave: rota ou padrão da rota
    // Valor: ID do SystemResource necessário para acessar
    public static readonly Dictionary<string, int[]> Rules = new()
        {
            { "/users", new[] { 2 } },

            { "/reports", new[] { 4 } },

            { "/resources", new[] { 3 } },

            // Outros endpoints podem ser adicionados aqui
            // { "/outro-endpoint", new[] { 5 } }
        };

    public static int[] GetRequiredPermissions(string path)
    {
      path = path.ToLower();
      foreach (var rule in Rules)
      {
        if (path.Contains(rule.Key))
          return rule.Value;
      }

      return Array.Empty<int>();
    }
  }
}
