using Ensek.TechExercise.Domain;
using Ensek.TechExercise.WebApi.Models;

namespace Ensek.TechExercise.WebApi.Services
{
    public interface IMeterReaderManager
    {
        Task<MeterReadingResponseModel> StoreMeterReadingsAsync(IEnumerable<MeterReading> meterReadings);
    }
}
