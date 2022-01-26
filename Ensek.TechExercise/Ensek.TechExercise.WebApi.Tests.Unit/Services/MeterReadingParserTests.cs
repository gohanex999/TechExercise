using Ensek.TechExercise.Domain;
using Ensek.TechExercise.WebApi.Models;
using Ensek.TechExercise.WebApi.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Ensek.TechExercise.WebApi.Tests.Unit.Services
{
    [TestFixture]
    public class MeterReadingParserTests
    {
        private MeterReadingParser _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new MeterReadingParser();
        }

        [Test]
        public void GivenTheMeterReadingsCsvHasValidData_WhenParseToObjectIsCalled_ThenReturnValidMeterReadings()
        {
            var meterReadingDateTimeString = "20/05/2019  09:24:00";

            var csv = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv() { AccountId = "1", MeterReadingDateTime = "20/05/2019  09:24:00", MeterReadValue = "1234"}
            };

            var expected = new List<MeterReading>()
            {
                new MeterReading() { AccountId = 1, MeterReadingDateTime = DateTime.Parse(meterReadingDateTimeString), MeterReadValue = 1234 }
            };

            var actual = _sut.ParseToMeterReadings(csv);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GivenTheMeterReadingsCsvHasInvalidData_WhenParseToObjectIsCalled_ThenReturnDefaultMeterReadings()
        {
            var csv = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv() { AccountId = "w", MeterReadingDateTime = "241220/05/2019  09:24:00", MeterReadValue = "xxxx"}
            };

            var expected = new List<MeterReading>()
            {
                new MeterReading()
            };

            var actual = _sut.ParseToMeterReadings(csv);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GivenTheMeterReadingsCsvHasExceeded5DigitsReadValueData_WhenParseToObjectIsCalled_ThenReturnZeroMeterReadValue()
        {
            var csv = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv() { AccountId = "w", MeterReadingDateTime = "241220/05/2019  09:24:00", MeterReadValue = "999999"}
            };

            var expected = new List<MeterReading>()
            {
                new MeterReading()
            };

            var actual = _sut.ParseToMeterReadings(csv);
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
