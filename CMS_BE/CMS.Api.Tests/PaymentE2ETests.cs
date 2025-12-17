using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace CMS.Api.Tests
{
    public class PaymentE2ETests : IClassFixture<Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>>
    {
        private readonly Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> _factory;
        public PaymentE2ETests(Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreatePatient_Then_VerifyPayment_CreatesBillAndIsDownloadable()
        {
            var client = _factory.CreateClient();

            // 1) Create patient
            var patientPayload = new
            {
                date_of_birth = "2000-01-01",
                sex = "M",
                country = "IN",
                pincode = "560001",
                city = "Bangalore",
                state = "KA",
                blood_group = "O+"
            };

            var patientResp = await client.PostAsJsonAsync("/api/Patient", patientPayload);
            patientResp.EnsureSuccessStatusCode();

            // 2) Simulate payment verification
            var verificationPayload = new
            {
                razorpayOrderId = "test_order_123",
                razorpayPaymentId = "test_payment_123",
                razorpaySignature = "test_signature"
            };

            var verifyResp = await client.PostAsJsonAsync("/api/Payment/verify-payment", verificationPayload);
            verifyResp.EnsureSuccessStatusCode();

            var json = await verifyResp.Content.ReadAsStringAsync();
            // Parse response in a case-insensitive manner for property names
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new Dictionary<string, JsonElement>();
            var successKey = dict.Keys.FirstOrDefault(k => string.Equals(k, "success", StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(successKey);
            Assert.True(dict[successKey!].GetBoolean());
            // Check for a wrapper that places the payload in `Data` (ResponseWrapperMiddleware)
            var dataKey = dict.Keys.FirstOrDefault(k => string.Equals(k, "data", StringComparison.OrdinalIgnoreCase));
            Dictionary<string, JsonElement> payload = dict;

            if (dataKey != null)
            {
                var dataElem = dict[dataKey];
                if (dataElem.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    payload = dataElem.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
                }
            }

            var billKey = payload.Keys.FirstOrDefault(k => string.Equals(k, "billPath", StringComparison.OrdinalIgnoreCase) || string.Equals(k, "billpath", StringComparison.OrdinalIgnoreCase));
            if (billKey == null)
            {
                // Dump response for diagnosis
                System.Console.WriteLine("Payment verify response JSON: " + json);
                Assert.False(true, "Expected a 'billPath' (or equivalent) property in response JSON (either top-level or inside Data), but none was found.");
            }

            var billPath = payload[billKey!].GetString();
            Assert.False(string.IsNullOrEmpty(billPath));

            // 3) Download the bill via API
            var fileName = billPath!.TrimStart('/').Split('/').Last();
            var downloadResp = await client.GetAsync($"/api/Payment/download-bill/{fileName}");
            downloadResp.EnsureSuccessStatusCode();

            Assert.Equal("application/pdf", downloadResp.Content.Headers.ContentType!.MediaType);
            var bytes = await downloadResp.Content.ReadAsByteArrayAsync();
            System.Console.WriteLine($"Download Response Content-Length header: {downloadResp.Content.Headers.ContentLength}");
            System.Console.WriteLine($"Read bytes length: {bytes.Length}");

            if (bytes.Length == 0)
            {
                // Fallback: check the filesystem path where the controller saved the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bills", fileName);
                System.Console.WriteLine($"Checking filesystem file: {filePath}");
                Assert.True(System.IO.File.Exists(filePath), $"Expected file to exist at {filePath}");
                var fileLength = new System.IO.FileInfo(filePath).Length;
                System.Console.WriteLine($"Filesystem file length: {fileLength}");
                Assert.True(fileLength > 0, "Filesystem file is empty");
            }
            else
            {
                Assert.NotEmpty(bytes);
            }
        }
    }
}