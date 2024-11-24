using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Theunis.Assesment.Web.Models
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; } = "";
        public string AccountNumber { get; set; } = "";

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; } = "";
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = "";
        public string OutputStatus { get; set; } = "";
    }
}
