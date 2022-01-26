using Ensek.TechExercise.Domain;
using Ensek.TechExercise.WebApi.Models;
using Ensek.TechExercise.WebApi.Repositories;
using Ensek.TechExercise.WebApi.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ensek.TechExercise.WebApi.Tests.Unit.Services
{
    [TestFixture]
    public class MeterReadingManagerTests
    {
        private MeterReadingManager _sut;
        private Mock<IMeterReadingRepository> _mockMeterReadingRepository;
        private Mock<IAccountRepository> _mockAccountRepository;

        [SetUp]
        public void Setup()
        {
            _mockMeterReadingRepository = new Mock<IMeterReadingRepository>();
            _mockAccountRepository = new Mock<IAccountRepository>();
            _sut = new MeterReadingManager(_mockMeterReadingRepository.Object, _mockAccountRepository.Object);
        }

        [Test]
        public async Task GivenAnAccountDoesNotExist_WhenStoreMeterReadingsAsyncIsCalled_ThenMeterReadingDoesNotGetStoredAndFailureCountIncrements()
        {
            _mockAccountRepository.Setup(x => x.AccountDoesNotExistAsync(It.IsAny<int>()))
                    .Returns(Task.FromResult(true));

            var meterReadings = new List<MeterReading>()
            {
                new MeterReading() { AccountId = 1234, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 4242 }
            };

            var expected = new MeterReadingResponseModel()
            {
                SuccessCount = 0,
                FailureCount = 1
            };

            var actual = await _sut.StoreMeterReadingsAsync(meterReadings);

            _mockMeterReadingRepository.Verify(x => x.StoreMeterReadingAsync(It.IsAny<Dtos.MeterReading>()), Times.Never);
            _mockMeterReadingRepository.Verify(x => x.SaveAsync(), Times.Never);
            actual.Should().BeEquivalentTo(expected);            
        }

        [Test]
        public async Task GivenTheLatestMeterReadingIsOlderThanTheLastRead_WhenStoreMeterReadingAsyncIsCalled_ThenMeterReadingDoesNotGetStoredAndFailureCountIncrements()
        {
            _mockAccountRepository.Setup(x => x.AccountDoesNotExistAsync(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            var lastMeterReadingDateTime = DateTime.UtcNow;
            var latestMeterReadingDateTime = DateTime.UtcNow.AddDays(-1);

            var meterReading = new Dtos.MeterReading()
            {
                MeterReadingId = 1212,
                AccountId = 9876,
                Account = new Dtos.Account()
                {
                    AccountId = 9876,
                    FirstName = "TestFirstName",
                    LastName = "TestSurname"
                },
                MeterReadingDateTime = latestMeterReadingDateTime
            };

            _mockMeterReadingRepository.Setup(x => x.LastMeterReadingForAccountAsync(It.IsAny<int>()))
                                       .Returns(Task.FromResult(meterReading));

            var meterReadings = new List<MeterReading>()
            {
                new MeterReading() { AccountId = 9876, MeterReadingDateTime = lastMeterReadingDateTime, MeterReadValue = 4242 }
            };

            var expected = new MeterReadingResponseModel()
            {
                SuccessCount = 0,
                FailureCount = 1
            };

            var actual = await _sut.StoreMeterReadingsAsync(meterReadings);

            _mockMeterReadingRepository.Verify(x => x.StoreMeterReadingAsync(It.IsAny<Dtos.MeterReading>()), Times.Never);
            _mockMeterReadingRepository.Verify(x => x.SaveAsync(), Times.Never);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GivenTheMeterReadingIsADuplicate_WhenStoreMeterReadingAsyncIsCalled_ThenMeterReadingDoesNotGetStoredAndFailureCountIncrements()
        {
            _mockAccountRepository.Setup(x => x.AccountDoesNotExistAsync(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            _mockMeterReadingRepository.Setup(x => x.LastMeterReadingForAccountAsync(It.IsAny<int>()))
                                       .Returns(Task.FromResult(default(Dtos.MeterReading)));

            _mockMeterReadingRepository.Setup(x => x.DuplicateMeterReadingAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                                       .Returns(Task.FromResult(true));

            var meterReadings = new List<MeterReading>()
            {
                new MeterReading() { AccountId = 9876, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 4242 }
            };

            var expected = new MeterReadingResponseModel()
            {
                SuccessCount = 0,
                FailureCount = 1
            };

            var actual = await _sut.StoreMeterReadingsAsync(meterReadings);

            _mockMeterReadingRepository.Verify(x => x.StoreMeterReadingAsync(It.IsAny<Dtos.MeterReading>()), Times.Never);
            _mockMeterReadingRepository.Verify(x => x.SaveAsync(), Times.Never);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GivenAnAccountExistsAndMeterReadingIsNew_WhenStoreMeterReadingAsyncIsCalled_ThenMeterReadingIsStoredAndSuccessCountIsIncremented()
        {
            _mockAccountRepository.Setup(x => x.AccountDoesNotExistAsync(It.IsAny<int>()))
                    .Returns(Task.FromResult(false));

            _mockMeterReadingRepository.SetupSequence(x => x.LastMeterReadingForAccountAsync(It.IsAny<int>()))
                                       .Returns(Task.FromResult(default(Dtos.MeterReading)));

            _mockMeterReadingRepository.Setup(x => x.DuplicateMeterReadingAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                                       .Returns(Task.FromResult(false));

            var meterReadings = new List<MeterReading>()
            {
                new MeterReading() { AccountId = 9876, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 4242 }
            };

            var expected = new MeterReadingResponseModel()
            {
                SuccessCount = 1,
                FailureCount = 0
            };

            var actual = await _sut.StoreMeterReadingsAsync(meterReadings);

            _mockMeterReadingRepository.Verify(x => x.StoreMeterReadingAsync(It.IsAny<Dtos.MeterReading>()), Times.Once);
            actual.Should().BeEquivalentTo(expected);
            _mockMeterReadingRepository.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task GivenASuccessfulAndAFailingMeterReadingStore_WhenStoreMeterReadingsAsyncIsCalled_ThenOneReadingIsStored()
        {
            _mockAccountRepository.SetupSequence(x => x.AccountDoesNotExistAsync(It.IsAny<int>()))
                    .Returns(Task.FromResult(false))
                    .Returns(Task.FromResult(true));

            _mockMeterReadingRepository.Setup(x => x.LastMeterReadingForAccountAsync(It.IsAny<int>()))
                                       .Returns(Task.FromResult(default(Dtos.MeterReading)));

            _mockMeterReadingRepository.Setup(x => x.DuplicateMeterReadingAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                                       .Returns(Task.FromResult(false));

            var meterReadings = new List<MeterReading>()
            {
                new MeterReading() { AccountId = 9876, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 4242 },
                new MeterReading() { AccountId = 1337, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 1234 }
            };

            var expected = new MeterReadingResponseModel()
            {
                SuccessCount = 1,
                FailureCount = 1
            };

            var actual = await _sut.StoreMeterReadingsAsync(meterReadings);

            _mockMeterReadingRepository.Verify(x => x.StoreMeterReadingAsync(It.IsAny<Dtos.MeterReading>()), Times.Once);
            _mockMeterReadingRepository.Verify(x => x.SaveAsync(), Times.Once);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
