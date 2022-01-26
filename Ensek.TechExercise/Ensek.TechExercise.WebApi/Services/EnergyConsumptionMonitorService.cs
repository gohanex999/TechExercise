using Ensek.TechExercise.WebApi.Models;
using Ensek.TechExercise.WebApi.Services;

namespace Ensek.TechExercise.Domain.Services
{
    public class EnergyConsumptionMonitorService : IEnergyConsumptionMonitorService
    {
        private readonly IMeterReaderManager _meterReaderManager;
        private readonly IMeterReadingParser _meterReadingParser;

        public EnergyConsumptionMonitorService(IMeterReaderManager meterReaderManager, IMeterReadingParser meterReadingParser)
        {
            _meterReaderManager = meterReaderManager;
            _meterReadingParser = meterReadingParser;
        }

        public async Task<MeterReadingResponseModel> ProcessMeterReadingsAsync(IFormFile file)
        {
            var meterReadingsCsv = _meterReadingParser.ParseCsv(file);
            var meterReadings = _meterReadingParser.ParseToMeterReadings(meterReadingsCsv);
            return await _meterReaderManager.StoreMeterReadingsAsync(meterReadings);
        }
    }
}
