using Microsoft.EntityFrameworkCore;
using Theunis.Assesment.Web.Controllers;
using Theunis.Assesment.Web.Data.Models;
using Theunis.Assesment.Web.Data.Repository.Interfaces;

namespace Theunis.Assesment.Web.Data.Repository
{
    public class AssesmentRepository: IAssesmentRepository
    {
        #region Constructor
        private readonly ILogger<AssesmentRepository> _logger;
        // Private readonly field for database context
        private readonly AssesmentDbContext _context;

        // Constructor that initializes the context
        public AssesmentRepository(ILogger<AssesmentRepository> logger, AssesmentDbContext context)
        {
            _logger = logger;
            _context = context; // Assigning the context to the private field
        }
        #endregion

        public async Task<List<Transaction>> GetTransactions()
        {
            List<Transaction> lstTansactions = new List<Transaction>();
            try
            {
                // LINQ query to select transactions from the database
                var transactionData = await (from o in _context.Transactions select o).ToListAsync();

                lstTansactions = transactionData; // Assigning the retrieved data to the DTO
            }
            catch (Exception ex)
            {
                // Log exception to the EventLog table in the database
                _logger.LogError(ex, "Class: AssesmentRepository - Method: GetTransactions");
            }
            return lstTansactions; // Returning the transaction list
        }
    }
}
