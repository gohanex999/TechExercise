using CsvHelper;
using Ensek.TechExercise.WebApi.Context;
using Ensek.TechExercise.WebApi.Dtos;
using System.Globalization;

namespace Ensek.TechExercise.DataAccess.Sql
{
    public static class DataSeeder
    {
        //private const string TestAccountCsvFilePath = "D:\\Projects\\Ensek\\TechExercise\\Ensek.TechExercise\\Ensek.TechExercise.DataAccess.Sql\\Resources\\Test_Accounts.csv";

        private const string AccountsCsv = "Test_Accounts.csv";
        
        public static void Seed(EnsekDbContext context)
        {

            var csvFilePath = Path.Combine(Environment.CurrentDirectory, AccountsCsv);

            context.Database.EnsureCreated();
            if (context.Accounts.Any()) return;

            var accounts = new List<Account>();

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                accounts = csv.GetRecords<Account>().ToList();
            }

            context.Accounts.AddRange(accounts);
            context.SaveChanges();
        }
    }
}