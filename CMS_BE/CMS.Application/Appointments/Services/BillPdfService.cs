using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CMS.Domain;

namespace CMS.Services
{
    public class BillPdfService
    {
        public byte[] GenerateBill(Payment payment)
        {
            Console.WriteLine($"=== BillPdfService GenerateBill ===");
            Console.WriteLine($"Amount: {payment.amount}");
            Console.WriteLine($"Original Amount: {payment.original_amount}");
            Console.WriteLine($"Discount Amount: {payment.discount_amount}");
            Console.WriteLine($"Is Followup: {payment.is_followup}");
            Console.WriteLine($"Will show discount: {payment.discount_amount > 0}");
            
            QuestPDF.Settings.License = LicenseType.Community;
            
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("CLINIC MANAGEMENT SYSTEM")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text("PAYMENT RECEIPT").Bold().FontSize(16);
                            
                            x.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Payment ID: {payment.payment_id}");
                                    var istTime = payment.created_at.AddHours(5.5); // Convert UTC to IST
                                    col.Item().Text($"Date: {istTime:dd/MM/yyyy HH:mm}");
                                    col.Item().Text($"Status: {payment.status.ToUpper()}");
                                });
                            });

                            x.Item().LineHorizontal(1);

                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Description").Bold();
                                    header.Cell().Element(CellStyle).Text("Amount").Bold();
                                });

                                table.Cell().Element(CellStyle).Text("Doctor Appointment Fee");
                                table.Cell().Element(CellStyle).Text($"₹{payment.original_amount}");

                                if (payment.discount_amount > 0)
                                {
                                    table.Cell().Element(CellStyle).Text("Follow-up Discount (30%)");
                                    table.Cell().Element(CellStyle).Text($"-₹{payment.discount_amount}").FontColor(Colors.Green.Medium);
                                }

                                table.Cell().Element(CellStyle).Text("Total Amount").Bold();
                                table.Cell().Element(CellStyle).Text($"₹{payment.amount}").Bold();
                            });

                            x.Item().Text($"Razorpay Payment ID: {payment.razorpay_payment_id}").FontSize(10);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Thank you for choosing our clinic services!");
                });
            }).GeneratePdf();
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
        }
    }
}