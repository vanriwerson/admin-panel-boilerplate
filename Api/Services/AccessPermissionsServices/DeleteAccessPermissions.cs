using Api.Data;
using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
  public class DeleteAccessPermissions
  {
    private readonly ApiDbContext _context;

    public DeleteAccessPermissions(ApiDbContext context)
    {
      _context = context;
    }

    public async Task ExecuteAsync(int userId)
    {
      var userPermissions = await _context.AccessPermissions
          .Where(ap => ap.UserId == userId)
          .ToListAsync();

      if (userPermissions.Any())
        _context.AccessPermissions.RemoveRange(userPermissions);
    }
  }
}
