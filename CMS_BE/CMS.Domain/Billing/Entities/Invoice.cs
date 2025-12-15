using System.ComponentModel.DataAnnotations;
using CMS.Domain.Billing.Enums;

namespace CMS.Domain.Billing.Entities
{
    public class Invoice
    {
        public Guid InvoiceID { get; set; }
        [Required]
        public Guid BillID { get; set; }
        [Required]
        public DateTime InvoiceDate { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public decimal PaidAmount { get; set; }
        [Required]
        public decimal BalanceAmount { get; set; }
        [Required]
        public PaymentStatusType PaymentStatus { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        public PaymentMethodTypes PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}