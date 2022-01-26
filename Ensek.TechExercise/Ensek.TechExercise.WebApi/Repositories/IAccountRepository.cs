namespace Ensek.TechExercise.WebApi.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> AccountDoesNotExistAsync(int accountId);
    }
}
