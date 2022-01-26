using Ensek.TechExercise.WebApi.Context;
using Ensek.TechExercise.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ensek.TechExercise.WebApi.Repositories
{

    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly EnsekDbContext _context;

        public MeterReadingRepository(EnsekDbContext context)
        {
            _context = context;
        }

        public async Task<MeterReading?> LastMeterReadingForAccountAsync(int accountId)
        {
            return await _context.MeterReadings.Where(x => x.AccountId == accountId)
                                    .OrderByDescending(x => x.MeterReadingDateTime)
                                    .FirstOrDefaultAsync();
        }

        public async Task<bool> DuplicateMeterReadingAsync(int accountId, DateTime meterReadingDateTime)
        {
            var duplicateReading = await _context.MeterReadings.Where(x => x.AccountId == accountId
                                                                && x.MeterReadingDateTime == meterReadingDateTime)
                                                         .FirstOrDefaultAsync();

            return duplicateReading != null;
        }

        public async Task StoreMeterReadingAsync(MeterReading meterReading)
        {
            await _context.MeterReadings.AddAsync(meterReading);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
