using Ensek.TechExercise.Domain;
using Ensek.TechExercise.WebApi.Models;

namespace Ensek.TechExercise.WebApi.Services
{
    public interface IMeterReadingParser
    {
        List<MeterReadingCsv> ParseCsv(IFormFile file);
        List<MeterReading> ParseToObjects(IEnumerable<MeterReadingCsv> meterReadingsCsv);
    }
}
