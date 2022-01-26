using Ensek.TechExercise.WebApi.Context;
using Microsoft.EntityFrameworkCore;

namespace Ensek.TechExercise.WebApi.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly EnsekDbContext _context;

        public AccountRepository(EnsekDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AccountDoesNotExistAsync(int accountId)
        {
            return !await _context.Accounts.AnyAsync(x => x.AccountId == accountId);
        }
    }
}
