using Ensek.TechExercise.WebApi.Context;
using Ensek.TechExercise.WebApi.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Ensek.TechExercise.WebApi.Tests.Integration.Repositories
{
    [TestFixture]
    public class AccountRepositoryTests
    {
        private EnsekDbContext _dbContext;
        private AccountRepository _sut;

        [SetUp]
        public void Setup()
        {
            _dbContext = CreateDbContext();
            _sut = new AccountRepository(_dbContext);
        }

        [Test]
        public async Task GivenThatAnAccountDoesNotExist_WhenAccountDoesNotExistAsyncIsCalled_ThenReturnTrue()
        {
            var actual = await _sut.AccountDoesNotExistAsync(1);
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
