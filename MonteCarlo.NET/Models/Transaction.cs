using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace MonteCarlo.NET.Models
{
    public class Transaction
    {
        [Required]
        [Column("IdTransakcji")]
        public int TransactionId { get; set; }
        [Required]
        [Column("KontoUzytkownikaId")]
        public string UserAccountId { get; set; }
        public virtual UserAccount UserAccount { get; set; }
        [Required]
        [Column("Data")]
        public DateTime Date { get; set; }
        [Required]
        [Column("Kwota")]
        public double Amount { get; set; }
        [Required]
        [MaxLength(50)]
        [Column("Typ")]
        public string Type { get; set; }
    }
}
