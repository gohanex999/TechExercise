using Ensek.TechExercise.Domain.Services;
using Ensek.TechExercise.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ensek.TechExercise.WebApi.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IEnergyConsumptionMonitorService _service;

        public CustomerController(IEnergyConsumptionMonitorService service)
        {
            _service = service;
        }

        [HttpPost("meter-reading-uploads")]
        public async Task<MeterReadingResponseModel> Post(IFormFile file)
        {
            return await _service.ProcessMeterReadingsAsync(file);
        }        
    }
}
