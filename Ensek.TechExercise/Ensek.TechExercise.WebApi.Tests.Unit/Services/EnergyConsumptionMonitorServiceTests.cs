using Ensek.TechExercise.Domain;
using Ensek.TechExercise.Domain.Services;
using Ensek.TechExercise.WebApi.Models;
using Ensek.TechExercise.WebApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ensek.TechExercise.WebApi.Tests.Unit.Services
{
    [TestFixture]
    public class EnergyConsumptionMonitorServiceTests
    {
        private Mock<IMeterReaderManager> _mockMeterReaderManager;
        private Mock<IMeterReadingParser> _mockMeterReadingParser;
        private Mock<IFormFile> _mockFormFile;
        private EnergyConsumptionMonitorService _sut;

        [SetUp]
        public void Setup()
        {
            _mockMeterReaderManager = new Mock<IMeterReaderManager>();
            _mockMeterReadingParser = new Mock<IMeterReadingParser>();
            _mockFormFile = new Mock<IFormFile>();
            _sut = new EnergyConsumptionMonitorService(_mockMeterReaderManager.Object, _mockMeterReadingParser.Object);
        }

        [Test]
        public async Task GivenACsvFileToBeProcessed_WhenProcessMeterReadingsAsyncIsCalled_ExpectedModelsAreUsed()
        {
            var meterReadingsCsv = new List<MeterReadingCsv>();

            _mockMeterReadingParser.Setup(x => x.ParseCsv(_mockFormFile.Object))
                              .Returns(meterReadingsCsv);

            var meterReadingObjects = new List<MeterReading>();

            _mockMeterReadingParser.Setup(x => x.ParseToMeterReadings(meterReadingsCsv))
                              .Returns(meterReadingObjects);

            var responseModel = new MeterReadingResponseModel();

            _mockMeterReaderManager.Setup(x => x.StoreMeterReadingsAsync(meterReadingObjects))
                                   .Returns(Task.FromResult(responseModel));

            var actual = await _sut.ProcessMeterReadingsAsync(_mockFormFile.Object);

            _mockMeterReadingParser.Verify(x => x.ParseCsv(_mockFormFile.Object));
            _mockMeterReadingParser.Verify(x => x.ParseToMeterReadings(meterReadingsCsv));
            _mockMeterReaderManager.Verify(x => x.StoreMeterReadingsAsync(meterReadingObjects));
            actual.Should().BeEquivalentTo(responseModel);
        }
    }
}
