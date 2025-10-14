using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Api.Interfaces;
using Api.Models;

namespace Api.Helpers
{
  public static class ValidateEntity
  {
    public static bool HasValidProperties<T>(object obj)
    {
      var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (var prop in props)
      {
        if (!obj.GetType().GetProperties().Any(p => p.Name == prop.Name))
          return false;
      }
      return true;
    }
  }
}
