using Ensek.TechExercise.Domain;
using Ensek.TechExercise.WebApi.Models;
using Ensek.TechExercise.WebApi.Repositories;

namespace Ensek.TechExercise.WebApi.Services
{
    public class MeterReadingManager : IMeterReaderManager
    {
        private IMeterReadingRepository _meterReadingRepository;
        private IAccountRepository _accountRepository;

        public MeterReadingManager(IMeterReadingRepository meterReadingRepository, IAccountRepository accountRepository)
        {
            _meterReadingRepository = meterReadingRepository;
            _accountRepository = accountRepository;
        }

        public async Task<MeterReadingResponseModel> StoreMeterReadingsAsync(IEnumerable<MeterReading> meterReadings)
        {
            var successfulReadings = new List<MeterReading>();
            var failedReadings = meterReadings.Where(x => x.AccountId == 0 ||
                                                          x.MeterReadValue == 0 ||
                                                          x.MeterReadingDateTime.Equals(default(DateTime)))
                                                          .ToList();

            var validMeterReadings = meterReadings.Where(x => x.MeterReadValue > 0).ToList();

            foreach (var meterReading in validMeterReadings)
            {
                if (await _accountRepository.AccountDoesNotExistAsync(meterReading.AccountId))
                {
                    failedReadings.Add(meterReading);
                    continue;
                }

                var lastMeterReadingForAccount = await _meterReadingRepository.LastMeterReadingForAccountAsync(meterReading.AccountId);
                if (LatestMeterReadingOlderThanLastRead(meterReading.MeterReadingDateTime, lastMeterReadingForAccount?.MeterReadingDateTime))
                {
                    failedReadings.Add(meterReading);
                    continue;
                }

                if (await _meterReadingRepository.DuplicateMeterReadingAsync(meterReading.AccountId, meterReading.MeterReadingDateTime))
                {
                    failedReadings.Add(meterReading);
                    continue;
                }

                var meterReadingDto = new Dtos.MeterReading()
                {
                    MeterReadingDateTime = meterReading.MeterReadingDateTime,
                    MeterReadingValue = meterReading.MeterReadValue,
                    AccountId = meterReading.AccountId
                };

                await _meterReadingRepository.StoreMeterReadingAsync(meterReadingDto);
                successfulReadings.Add(meterReading);
            }

            if (successfulReadings.Any())
            {
                await _meterReadingRepository.SaveAsync();
            }

            return new MeterReadingResponseModel
            {
                SuccessCount = successfulReadings.Count,
                FailureCount = failedReadings.Count
            };
        }

        private static bool LatestMeterReadingOlderThanLastRead(DateTime latestMeterReading, DateTime? lastMeterReading)
        {
            return latestMeterReading > lastMeterReading;
        }
    }
}
