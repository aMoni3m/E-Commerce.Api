using E_Commerce.Api.Models;
using E_Commerce.Api.Services.Interfaces;

namespace E_Commerce.Api.Helpers
{
    public class RefundEmailHelper
    {
        private readonly IEmailService _emailService;

        public RefundEmailHelper(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendRefundSuccessEmailAsync(Refund refund, string orderNumber, Cancellation cancellation, string customerEmail)
        {
            if (string.IsNullOrEmpty(customerEmail))
            {
                return;
            }

            string subject = $"Your Refund Has Been Processed Successfully, Order #{orderNumber}";
            string emailBody = GenerateRefundSuccessEmailBody(refund, orderNumber, cancellation);

            await _emailService.SendEmailAsync(customerEmail, subject, emailBody, isBodyHtml: true);
        }

        private string GenerateRefundSuccessEmailBody(Refund refund, string orderNumber, Cancellation cancellation)
        {
            var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

            string completedAtStr = refund.CompletedAt.HasValue
                ? TimeZoneInfo.ConvertTimeFromUtc(refund.CompletedAt.Value, istZone).ToString("dd MMM yyyy HH:mm:ss")
                : "N/A";

            return $@"
            <html>
            <body style='font-family: Arial, sans-serif; margin: 0; padding: 0;'>
                <div style='background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border: 1px solid #ddd;'>
                        <div style='padding: 20px; text-align: center; background-color: #2E86C1; color: #ffffff;'>
                            <h2>Your Refund is Complete</h2>
                        </div>
                        <div style='padding: 20px;'>
                            <p>Dear Customer,</p>
                            <p>Your refund has been processed successfully. Below are the details:</p>
                            <table style='width: 100%; border-collapse: collapse;'>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Order Number</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{orderNumber}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Refund Transaction ID</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{refund.TransactionId}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Order Amount</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>₹{cancellation.OrderAmount}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Cancellation Charges</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>₹{cancellation.CancellationCharges ?? 0.00m}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Cancellation Reason</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{cancellation.Reason}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Refunded Method</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{refund.RefundMethod}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>Refunded Amount</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>₹{refund.Amount}</td>
                                </tr>
                                <tr>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>CompletedAt At</td>
                                    <td style='border: 1px solid #ddd; padding: 8px;'>{completedAtStr}</td>
                                </tr>
                            </table>
                            <p>Thank you for shopping with us.</p>
                            <p>Best regards,<br/>The ECommerce Team</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}
