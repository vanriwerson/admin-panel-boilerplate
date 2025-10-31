using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using System;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Api.Validations
{
  public static class ValidateEntity
  {
    public static void HasExpectedProperties<T>(object obj)
    {
      if (obj == null)
        throw new AppException("Objeto inválido fornecido.");

      var expectedProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      var objProps = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

      foreach (var expectedProp in expectedProps)
      {
        if (!objProps.Any(p => p.Name == expectedProp.Name))
        {
          throw new AppException($"Propriedade obrigatória '{expectedProp.Name}' não encontrada no objeto '{obj.GetType().Name}'.");
        }
      }
    }

    public static void HasExpectedValues<T>(object obj)
    {
      if (obj == null)
        throw new AppException("Objeto inválido fornecido.");

      var expectedProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      var objType = obj.GetType();

      foreach (var expectedProp in expectedProps)
      {
        var objProp = objType.GetProperty(expectedProp.Name);
        if (objProp == null) continue;

        var value = objProp.GetValue(obj);

        if (value == null) continue;

        var expectedType = Nullable.GetUnderlyingType(expectedProp.PropertyType) ?? expectedProp.PropertyType;
        var actualType = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();

        if (!expectedType.IsAssignableFrom(actualType))
        {
          throw new AppException(
              $"Campo '{expectedProp.Name}' com dado inválido. Esperado: '{expectedType.Name}', Recebido: '{actualType.Name}'."
          );
        }

        if (expectedType == typeof(string) && string.IsNullOrWhiteSpace((string)value))
          throw new AppException($"'{expectedProp.Name}' não pode ser vazio.");

        if (expectedType == typeof(int) && (int)value < 0)
          throw new AppException($"'{expectedProp.Name}' não pode conter valor negativo.");

        if (expectedType == typeof(Guid) && (Guid)value == Guid.Empty)
          throw new AppException($"'{expectedProp.Name}' não pode conter GUID vazio.");
      }
    }

    public static async Task<T> EnsureEntityExistsAsync<T>(
        IGenericRepository<T> repository,
        int id,
        string? entityName = null) where T : class
    {
      if (id <= 0)
        throw new AppException("Id inválido.");

      var entity = await repository.GetByIdAsync(id);

      if (entity == null)
      {
        var name = entityName ?? typeof(T).Name;
        throw new AppException($"{name} não encontrado(a).", (int)HttpStatusCode.NotFound);
      }

      return entity;
    }
  }
}
