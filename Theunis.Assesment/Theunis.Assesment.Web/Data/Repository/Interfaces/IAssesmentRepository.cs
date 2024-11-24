using Theunis.Assesment.Web.Data.Models;

namespace Theunis.Assesment.Web.Data.Repository.Interfaces
{
    public interface IAssesmentRepository
    {
        Task<List<Transaction>> GetTransactions();
        void AddTransactions(List<Transaction> lstTransactions);
    }
}
