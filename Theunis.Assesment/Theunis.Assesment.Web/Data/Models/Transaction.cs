using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Theunis.Assesment.Web.Data.Models
{
    [Table("Transaction", Schema = "dbo")]
    public partial class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionId { get; set; } = "";

        [Required]
        [StringLength(30)]
        public string AccountNumber { get; set; } = "";

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string CurrencyCode { get; set; } = "";

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; } = "";

        [Required]
        [StringLength(1)]
        public string OutputStatus { get; set; } = "";
    }
    }
