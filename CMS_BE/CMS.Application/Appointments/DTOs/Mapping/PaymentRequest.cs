namespace CMS.Models
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsFollowup { get; set; }
        public string Currency { get; set; } = "INR";
        public string PatientId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PaymentResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class PaymentVerification
    {
        public string razorpayOrderId { get; set; } = string.Empty;
        public string razorpayPaymentId { get; set; } = string.Empty;
        public string razorpaySignature { get; set; } = string.Empty;
    }
}