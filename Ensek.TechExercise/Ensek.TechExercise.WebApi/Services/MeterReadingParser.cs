using CsvHelper;
using Ensek.TechExercise.Domain;
using Ensek.TechExercise.WebApi.Models;
using System.Globalization;

namespace Ensek.TechExercise.WebApi.Services
{
    public class MeterReadingParser : IMeterReadingParser
    {
        private const int FiveDigitMeterReadingLimit = 99999;

        public List<MeterReadingCsv> ParseCsv(IFormFile file)
        {
            var reader = new StreamReader(file.OpenReadStream());
            var csv = new CsvReader(reader, CultureInfo.GetCultureInfo("en-GB"));
            return csv.GetRecords<MeterReadingCsv>().ToList();
        }

        public List<MeterReading> ParseToObjects(IEnumerable<MeterReadingCsv> meterReadingsCsv)
        {
            List<MeterReading> meterReadings = new List<MeterReading>();

            foreach (var meterReadingCsv in meterReadingsCsv)
            {
                var meterReading = new MeterReading();

                if (int.TryParse(meterReadingCsv.AccountId, out int id))
                {
                    meterReading.AccountId = id;
                }
                if (DateTime.TryParse(meterReadingCsv.MeterReadingDateTime, out DateTime dateTime))
                {
                    meterReading.MeterReadingDateTime = dateTime;
                }
                if (int.TryParse(meterReadingCsv.MeterReadValue, out int value) && value < FiveDigitMeterReadingLimit)
                {
                    meterReading.MeterReadValue = value;
                }

                meterReadings.Add(meterReading);
            }

            return meterReadings;
        }
    }
}
