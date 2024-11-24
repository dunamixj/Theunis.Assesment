namespace Theunis.Assesment.Web.Models
{
    public class CSVTransactionModel
    {
        public List<CsvTransaction> Transactions { get; set; }
    }
    public class CsvTransaction
    {
        public string TransactionIdentifier { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
    }
}
