using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ensek.TechExercise.WebApi.Dtos
{
    [Table(nameof(MeterReading))]
    [Index(nameof(AccountId))]
    [Index(nameof(AccountId), nameof(MeterReadingDateTime))]
    public class MeterReading
    {
        [Key]
        public int MeterReadingId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadingValue { get; set; }

        [ForeignKey(nameof(Account))]
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
