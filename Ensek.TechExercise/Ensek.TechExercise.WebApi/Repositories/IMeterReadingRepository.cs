using Ensek.TechExercise.WebApi.Dtos;

namespace Ensek.TechExercise.WebApi.Repositories
{
    public interface IMeterReadingRepository
    {
        Task<MeterReading?> LastMeterReadingForAccountAsync(int accountId);
        Task<bool> DuplicateMeterReadingAsync(int accountId, DateTime meterReadingDateTime);
        Task StoreMeterReadingAsync(MeterReading meterReading);
        Task SaveAsync();
    }
}