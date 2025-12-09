using E_Commerce.Api.Data;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.PaymentDTOs;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IEmailService _emailService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ApplicationDbContext _context;

        public PaymentService(IEmailService emailService, IPaymentRepository paymentRepository, ApplicationDbContext context)
        {
            _emailService = emailService;
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public async Task<ApiResponse<PaymentResponseDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest)
        {
            // Use a transaction to guarantee atomic operations on Order and Payment
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Retrieve the order along with any existing payment record
                    var order = await _paymentRepository.GetOrderWithPaymentAsync(paymentRequest.OrderId, paymentRequest.CustomerId);

                    if (order == null)
                    {
                        return new ApiResponse<PaymentResponseDTO>(404, "Order not found.");
                    }

                    if (Math.Round(paymentRequest.Amount, 2) != Math.Round(order.TotalAmount, 2))
                    {
                        return new ApiResponse<PaymentResponseDTO>(400, "Payment amount does not match the order total.");
                    }

                    Payment payment;

                    // Check if an existing payment record is present
                    if (order.Payment != null)
                    {
                        // Allow retry only if previous payment failed and order status is still Pending
                        if (order.Payment.Status == PaymentStatus.Failed && order.OrderStatus == OrderStatus.Pending)
                        {
                            // Retry: update the existing payment record with new details
                            payment = order.Payment;
                            payment.PaymentMethod = paymentRequest.PaymentMethod;
                            payment.Amount = paymentRequest.Amount;
                            payment.PaymentDate = DateTime.UtcNow;
                            payment.Status = PaymentStatus.Pending;
                            payment.TransactionId = null; // Clear previous transaction id if any
                            await _paymentRepository.UpdatePaymentAsync(payment);
                        }
                        else
                        {
                            return new ApiResponse<PaymentResponseDTO>(400, "Order already has an associated payment.");
                        }
                    }
                    else
                    {
                        // Create a new Payment record if none exists
                        payment = new Payment
                        {
                            OrderId = paymentRequest.OrderId,
                            PaymentMethod = paymentRequest.PaymentMethod,
                            Amount = paymentRequest.Amount,
                            PaymentDate = DateTime.UtcNow,
                            Status = PaymentStatus.Pending
                        };

                        await _paymentRepository.CreatePaymentAsync(payment);
                    }

                    // For non-COD payments, simulate payment processing
                    if (!IsCashOnDelivery(paymentRequest.PaymentMethod))
                    {
                        var simulatedStatus = await SimulatePaymentGateway();
                        payment.Status = simulatedStatus;
                        if (simulatedStatus == PaymentStatus.Completed)
                        {
                            // Update the Transaction Id on successful payment
                            payment.TransactionId = GenerateTransactionId();
                            // Update order status accordingly
                            order.OrderStatus = OrderStatus.Processing;
                            await _paymentRepository.UpdateOrderAsync(order);
                        }
                    }
                    else
                    {
                        // For COD, mark the order status as Processing immediately
                        order.OrderStatus = OrderStatus.Processing;
                        await _paymentRepository.UpdateOrderAsync(order);
                    }

                    await _paymentRepository.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Send Order Confirmation Email if Order Status is Processing
                    // It means the user is either selected COD of the Payment is Sucessful 
                    if (order.OrderStatus == OrderStatus.Processing)
                    {
                        await SendOrderConfirmationEmailAsync(paymentRequest.OrderId);
                    }

                    // Manual mapping to PaymentResponseDTO
                    var paymentResponse = new PaymentResponseDTO
                    {
                        PaymentId = payment.Id,
                        OrderId = payment.OrderId,
                        PaymentMethod = payment.PaymentMethod,
                        TransactionId = payment.TransactionId,
                        Amount = payment.Amount,
                        PaymentDate = payment.PaymentDate,
                        Status = payment.Status
                    };

                    return new ApiResponse<PaymentResponseDTO>(200, paymentResponse);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while processing the payment.");
                }
            }
        }

        #region Helper Methods

        private bool IsCashOnDelivery(string paymentMethod)
        {
            return paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase) ||
                   paymentMethod.Equals("CashOnDelivery", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<PaymentStatus> SimulatePaymentGateway()
        {
            // Simulate payment gateway processing delay
            await Task.Delay(100);

            // Simulate 80% success rate
            var random = new Random();
            return random.Next(1, 101) <= 80 ? PaymentStatus.Completed : PaymentStatus.Failed;
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss")}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        private async Task SendOrderConfirmationEmailAsync(int orderId)
        {
            try
            {
                // Get order details for email
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order?.Customer != null)
                {
                    var subject = $"Order Confirmation - Order #{order.OrderNumber}";
                    var body = $@"
                        <html>
                        <body>
                            <h2>Order Confirmation</h2>
                            <p>Dear {order.Customer.FirstName},</p>
                            <p>Thank you for your order! Your order #{order.OrderNumber} has been confirmed and is being processed.</p>
                            <p>Order Date: {order.OrderDate:yyyy-MM-dd HH:mm:ss}</p>
                            <p>Total Amount: ${order.TotalAmount:F2}</p>
                            <p>We will send you another email once your order has been shipped.</p>
                            <p>Best regards,<br/>E-Commerce Team</p>
                        </body>
                        </html>";

                    await _emailService.SendEmailAsync(order.Customer.Email, subject, body, isBodyHtml: true);
                }
            }
            catch (Exception)
            {
                // Log error but don't fail the payment processing
                // Email sending failure should not affect payment success
            }
        }

        #endregion
    }
}