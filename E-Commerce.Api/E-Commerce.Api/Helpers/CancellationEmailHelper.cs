using E_Commerce.Api.Models;
using E_Commerce.Api.Services.Interfaces;

namespace E_Commerce.Api.Helpers
{
    public class CancellationEmailHelper
    {
        private readonly IEmailService _emailService;

        public CancellationEmailHelper(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task NotifyCancellationAcceptedAsync(Cancellation cancellation)
        {
            if (cancellation.Order == null || cancellation.Order.Customer == null)
            {
                return;
            }

            string subject = $"Cancellation Request Update - Order #{cancellation.Order.OrderNumber}";

            string emailBody = $@"
            <html>
              <head>
                <meta charset='UTF-8'>
              </head>
              <body style='font-family: Arial, sans-serif; background-color: #f0f8ff; margin: 0; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 20px; border: 1px solid #cccccc;'>
                  <div style='background-color: #dc3545; padding: 15px; text-align: center; color: #ffffff;'>
                    <h2 style='margin: 0;'>Cancellation Request {cancellation.Status}</h2>
                  </div>
                  <p style='margin: 20px 0 5px 0;'>Dear {cancellation.Order.Customer.FirstName} {cancellation.Order.Customer.LastName},</p>
                  <p style='margin: 5px 0 20px 0;'>Your cancellation request for Order <strong>#{cancellation.Order.OrderNumber}</strong> has been <span style='color: #dc3545; font-weight: bold;'>{cancellation.Status}</span>.</p>
                  <h3 style='color: #dc3545; border-bottom: 2px solid #eeeeee; padding-bottom: 5px;'>Cancellation Details</h3>
                  <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Order Number:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{cancellation.Order.OrderNumber}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Cancellation Reason:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{cancellation.Reason}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Admin Remark:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{cancellation.Remarks}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Requested At:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{cancellation.RequestedAt:MMMM dd, yyyy HH:mm}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Processed At:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{(cancellation.ProcessedAt.HasValue ? cancellation.ProcessedAt.Value.ToString("MMMM dd, yyyy HH:mm") : "N/A")}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Order Amount:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{cancellation.OrderAmount}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Cancellation Charges:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{cancellation.CancellationCharges}</td>
                    </tr>
                    <tr>
                      <td style='padding: 8px; background-color: #f8f8f8; border: 1px solid #dddddd;'><strong>Amount to be Refunded:</strong></td>
                      <td style='padding: 8px; border: 1px solid #dddddd;'>{cancellation.OrderAmount - (cancellation.CancellationCharges ?? 0)}</td>
                    </tr>
                  </table>
                  <div style='background-color:#f1f3f5; padding:15px; text-align:center; font-size:14px; color:#6c757d; margin-top:20px;'>
                    <p style='margin:0;'>Thank you for choosing Our E-Commerce Store.</p>
                  </div>
                </div>
              </body>
            </html>";

            await _emailService.SendEmailAsync(cancellation.Order.Customer.Email, subject, emailBody, isBodyHtml: true);
        }

        public async Task NotifyCancellationRejectionAsync(Cancellation cancellation)
        {
            if (cancellation.Order == null || cancellation.Order.Customer == null)
            {
                return;
            }

            string subject = $"Cancellation Request Rejected - Order #{cancellation.Order.OrderNumber}";

            string emailBody = $@"
            <html>
              <head>
                <meta charset='UTF-8'>
              </head>
              <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; margin: 0; padding: 20px;'>
                <div style='max-width:600px; margin:auto; background-color:#ffffff; padding:20px; border-radius:8px; box-shadow:0 4px 8px rgba(0,0,0,0.1); overflow:hidden;'>
                  <div style='background-color:#ffc107; padding:20px; text-align:center;'>
                    <h2 style='margin:0; color:#212529; font-size:26px;'>Cancellation Request Rejected</h2>
                  </div>
                  <div style='padding:20px; color:#343a40;'>
                    <p style='margin:15px 0; line-height:1.6;'>Dear {cancellation.Order.Customer.FirstName} {cancellation.Order.Customer.LastName},</p>
                    <p style='margin:15px 0; line-height:1.6;'>
                      We regret to inform you that your cancellation request for Order <strong>#{cancellation.Order.OrderNumber}</strong> has been 
                      <strong style='color:#dc3545;'>Rejected</strong>.
                    </p>
                    <h3 style='color:#dc3545; margin-bottom:10px;'>Rejection Details</h3>
                    <table style='width:100%; border-collapse:collapse; margin:20px 0;'>
                      <tr>
                        <th style='border:1px solid #dee2e6; padding:12px; text-align:left; background-color:#e9ecef;'>Order Number</th>
                        <td style='border:1px solid #dee2e6; padding:12px; text-align:left;'>{cancellation.Order.OrderNumber}</td>
                      </tr>
                      <tr>
                        <th style='border:1px solid #dee2e6; padding:12px; text-align:left; background-color:#e9ecef;'>Cancellation Reason</th>
                        <td style='border:1px solid #dee2e6; padding:12px; text-align:left;'>{cancellation.Reason}</td>
                      </tr>
                      <tr>
                        <th style='border:1px solid #dee2e6; padding:12px; text-align:left; background-color:#e9ecef;'>Rejection Reason</th>
                        <td style='border:1px solid #dee2e6; padding:12px; text-align:left;'>{cancellation.Remarks}</td>
                      </tr>
                      <tr>
                        <th style='border:1px solid #dee2e6; padding:12px; text-align:left; background-color:#e9ecef;'>Requested At</th>
                        <td style='border:1px solid #dee2e6; padding:12px; text-align:left;'>{cancellation.RequestedAt:MMMM dd, yyyy HH:mm}</td>
                      </tr>
                      <tr>
                        <th style='border:1px solid #dee2e6; padding:12px; text-align:left; background-color:#e9ecef;'>Processed At</th>
                        <td style='border:1px solid #dee2e6; padding:12px; text-align:left;'>{(cancellation.ProcessedAt.HasValue ? cancellation.ProcessedAt.Value.ToString("MMMM dd, yyyy HH:mm") : "N/A")}</td>
                      </tr>
                    </table>
                    <p style='margin:15px 0; line-height:1.6;'>If you have any questions or need further clarification, please do not hesitate to contact our support team.</p>
                    <a href='mailto:info@example.com' style='display:inline-block; padding:12px 24px; margin-top:20px; background-color:#dc3545; color:#ffffff; text-decoration:none; border-radius:4px; font-weight:bold;'>Contact Support</a>
                  </div>
                  <div style='background-color:#f1f3f5; padding:15px; text-align:center; font-size:14px; color:#6c757d; margin-top:20px;'>
                    <p style='margin:0;'>Thank you for choosing Our E-Commerce Store.</p>
                  </div>
                </div>
              </body>
            </html>";

            await _emailService.SendEmailAsync(cancellation.Order.Customer.Email, subject, emailBody, isBodyHtml: true);
        }
    }
}