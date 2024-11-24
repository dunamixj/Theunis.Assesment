using Microsoft.EntityFrameworkCore;
using Theunis.Assesment.Web.Data.Models;

namespace Theunis.Assesment.Web.Data
{
    public partial class AssesmentDbContext: DbContext
    {
        public AssesmentDbContext()
        {
        }

        public AssesmentDbContext(DbContextOptions<AssesmentDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Transaction> Transactions { get; set; }
    }
}

