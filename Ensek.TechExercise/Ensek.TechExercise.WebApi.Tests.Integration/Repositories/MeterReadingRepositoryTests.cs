using Ensek.TechExercise.Domain;
using Ensek.TechExercise.WebApi.Context;
using Ensek.TechExercise.WebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Ensek.TechExercise.WebApi.Dtos;
using System.Threading.Tasks;
using FluentAssertions;

namespace Ensek.TechExercise.WebApi.Tests.Integration.Repositories
{
    [TestFixture]
    public class MeterReadingRepositoryTests
    {
        private EnsekDbContext _dbContext;
        private MeterReadingRepository _sut;

        [SetUp]
        public void Setup()
        {
            _dbContext = CreateDbContext();
            _sut = new MeterReadingRepository(_dbContext);
        }

        [Test]
        public async Task GivenAccountHasAPreviousMeterReadings_WhenLastMeterReadingForAccountAsyncCalled_ThenTheLatestMeterReadingIsReturned()
        {
            // Setup
            var date1 = DateTime.UtcNow.AddDays(-3);
            var date2 = DateTime.UtcNow.AddDays(-2);
            var date3 = DateTime.UtcNow.AddDays(-1);

            var account = new Account()
            {
                AccountId = 1,
                FirstName = "TestFirstName",
                LastName = "TestSurname"
            };

            _dbContext.Accounts.Add(account);            

            var meterReadings = new List<Dtos.MeterReading>()
            {
                new Dtos.MeterReading() { AccountId = 1, MeterReadingDateTime = date1, MeterReadingValue = 1234 },
                new Dtos.MeterReading() { AccountId = 1, MeterReadingDateTime = date2, MeterReadingValue = 2345 },
                new Dtos.MeterReading() { AccountId = 1, MeterReadingDateTime = date3, MeterReadingValue = 3456 },
            };

            _dbContext.MeterReadings.AddRange(meterReadings);
            _dbContext.SaveChanges();

            var expected = new Dtos.MeterReading()
            {
                AccountId = 1,
                Account = account,
                MeterReadingDateTime = date3,
                MeterReadingValue = 3456
            };

            var actual = await _sut.LastMeterReadingForAccountAsync(1);

            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.MeterReadingId));

        }

        [Test]
        public async Task GivenAMeterReadingHasAlreadyBeenStored_WhenDuplicateMeterReadingAsyncIsCalled_ThenReturnTrueResult()
        {
            var dateTime = DateTime.UtcNow;

            var account = new Account()
            {
                AccountId = 1,
                FirstName = "TestFirstName",
                LastName = "TestSurname"
            };

            _dbContext.Accounts.Add(account);

            var meterReading = new Dtos.MeterReading() { AccountId = 1, MeterReadingDateTime = dateTime, MeterReadingValue = 1234 };

            _dbContext.MeterReadings.Add(meterReading);
            _dbContext.SaveChanges();

            var actual = await _sut.DuplicateMeterReadingAsync(account.AccountId, dateTime);
            Assert.IsTrue(actual);
        }

        private EnsekDbContext CreateDbContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<EnsekDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new EnsekDbContext(dbContextOptions);
        }
    }    
}
