using Ensek.TechExercise.WebApi.Models;

namespace Ensek.TechExercise.Domain.Services
{
    public interface IEnergyConsumptionMonitorService
    {
        Task<MeterReadingResponseModel> ProcessMeterReadingsAsync(IFormFile file);
    }
}
