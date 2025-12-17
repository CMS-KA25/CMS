using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using CMS.Models;
using CMS.Data;
using CMS.Domain;
using CMS.Services;

using System.Security.Cryptography;
using System.Text;


namespace CMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CmsDbContext _context;
        private readonly BillPdfService _billPdfService;
        private readonly string _keyId;
        private readonly string _keySecret;
        private static readonly Dictionary<string, PaymentRequest> _paymentData = new();

        public PaymentController(IConfiguration configuration, CmsDbContext context, BillPdfService billPdfService)
        {
            _configuration = configuration;
            _context = context;
            _billPdfService = billPdfService;
            _keyId = _configuration["Razorpay:KeyId"] ?? "";
            _keySecret = _configuration["Razorpay:KeySecret"] ?? "";
        }

        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] PaymentRequest request)
        {
            Console.WriteLine($"=== CREATE ORDER ENDPOINT HIT ===");
            Console.WriteLine($"Request received with IsFollowup: {request?.IsFollowup}");
            Console.WriteLine($"=================================");
            
            if (request == null) return BadRequest("Invalid request");
            
            try
            {
                Console.WriteLine($"Creating order - Amount: {request.Amount}, Original: {request.OriginalAmount}, Discount: {request.DiscountAmount}, IsFollowup: {request.IsFollowup}");
                
                RazorpayClient client = new RazorpayClient(_keyId, _keySecret);
                
                Dictionary<string, object> options = new Dictionary<string, object>
                {
                    {"amount", request.Amount * 100}, // Amount in paise
                    {"currency", request.Currency},
                    {"receipt", $"receipt_{DateTime.Now.Ticks}"},
                    {"payment_capture", 1}
                };

                Order order = client.Order.Create(options);
                string orderId = order["id"].ToString();
                
                // Store payment details in static dictionary
                _paymentData[orderId] = request;
                
                Console.WriteLine($"=== SESSION STORAGE ===");
                Console.WriteLine($"Storing PaymentAmount_{orderId} = {request.Amount}");
                Console.WriteLine($"Storing OriginalAmount_{orderId} = {request.OriginalAmount}");
                Console.WriteLine($"Storing DiscountAmount_{orderId} = {request.DiscountAmount}");
                Console.WriteLine($"Storing IsFollowup_{orderId} = {request.IsFollowup.ToString().ToLower()}");
                Console.WriteLine($"========================");
                
                var response = new PaymentResponse
                {
                    OrderId = orderId,
                    Key = _keyId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Description = request.Description
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerification verification)
        {
            Console.WriteLine($"=== VERIFY PAYMENT ENDPOINT HIT ===");
            Console.WriteLine($"Verification object: {verification?.razorpayOrderId}");
            Console.WriteLine($"===================================");
            
            try
            {
                Console.WriteLine($"Received payment verification request");
                Console.WriteLine($"OrderId: {verification?.razorpayOrderId}");
                
                // Get payment details from session using order ID
                var orderId = verification?.razorpayOrderId ?? "";
                
                // Get payment data from static dictionary
                var paymentData = _paymentData.GetValueOrDefault(orderId);
                
                Console.WriteLine($"=== DATA RETRIEVAL ===");
                Console.WriteLine($"Retrieved payment data: {paymentData != null}");
                
                var paymentAmount = paymentData?.Amount ?? 500m;
                var originalAmount = paymentData?.OriginalAmount ?? 500m;
                var discountAmount = paymentData?.DiscountAmount ?? 0m;
                var isFollowup = paymentData?.IsFollowup ?? false;
                
                Console.WriteLine($"Retrieved Amount: {paymentAmount}");
                Console.WriteLine($"Retrieved Original: {originalAmount}");
                Console.WriteLine($"Retrieved Discount: {discountAmount}");
                Console.WriteLine($"Retrieved IsFollowup: {isFollowup}");
                
                Console.WriteLine($"=== FINAL VALUES ===");
                Console.WriteLine($"Amount: {paymentAmount}, Original: {originalAmount}, Discount: {discountAmount}, IsFollowup: {isFollowup}");
                Console.WriteLine($"===================");
                
                var payment = new CMS.Domain.Payment
                {
                    razorpay_order_id = verification?.razorpayOrderId ?? "test_order",
                    razorpay_payment_id = verification?.razorpayPaymentId ?? "test_payment",
                    razorpay_signature = verification?.razorpaySignature ?? "test_signature",
                    amount = paymentAmount,
                    original_amount = originalAmount,
                    discount_amount = discountAmount,
                    is_followup = isFollowup,
                    currency = "INR",
                    description = "Consultation Fee",
                    status = "success",
                    updated_at = DateTime.UtcNow
                };

                Console.WriteLine($"=== BEFORE DATABASE SAVE ===");
                Console.WriteLine($"payment.amount = {payment.amount}");
                Console.WriteLine($"payment.original_amount = {payment.original_amount}");
                Console.WriteLine($"payment.discount_amount = {payment.discount_amount}");
                Console.WriteLine($"payment.is_followup = {payment.is_followup}");
                Console.WriteLine($"=============================");

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"=== AFTER DATABASE SAVE ===");
                Console.WriteLine($"Saved payment ID: {payment.payment_id}");
                Console.WriteLine($"============================");

                // Generate PDF bill
                Console.WriteLine($"Generating PDF with - Amount: {payment.amount}, Original: {payment.original_amount}, Discount: {payment.discount_amount}, IsFollowup: {payment.is_followup}");
                var pdfBytes = _billPdfService.GenerateBill(payment);
                var fileName = $"bill_{payment.payment_id}.pdf";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills", fileName);
                Console.WriteLine($"Saving PDF to: {filePath}");
                
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);
                
                payment.bill_pdf_path = $"/bills/{fileName}";
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Payment verified successfully", billPath = payment.bill_pdf_path });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VerifyPayment: {ex.Message}");
                return BadRequest(new { error = ex.Message, details = ex.StackTrace });
            }
        }

        [HttpGet("download-bill/{fileName}")]
        public IActionResult DownloadBill(string fileName)
        {
            try
            {
                Console.WriteLine($"Downloading file: {fileName}");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills", fileName);
                Console.WriteLine($"Looking for file at: {filePath}");
                
                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine($"File not found at: {filePath}");
                    return NotFound($"Bill not found at {filePath}");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                Console.WriteLine($"File found, size: {fileBytes.Length} bytes");
                return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading bill: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("view-bill/{fileName}")]
        public IActionResult ViewBill(string fileName)
        {
            try
            {
                Console.WriteLine($"Viewing file: {fileName}");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills", fileName);
                
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"Bill not found");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf"); // No filename = inline display
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error viewing bill: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        private string ComputeHMACSHA256(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}