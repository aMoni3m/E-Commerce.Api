using E_Commerce.Api.Data;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.PaymentDTOs;
using E_Commerce.Api.Helpers;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IEmailService _emailService;

        private readonly PaymentEmailHelper _emailHelper;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IEmailService emailService, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;

            _emailHelper = new PaymentEmailHelper(_emailService, context);
        }

        public async Task<ApiResponse<PaymentResponseDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = await _unitOfWork.Payments.GetOrderWithPaymentAsync(paymentRequest.OrderId, paymentRequest.CustomerId);

                if (order == null)
                {
                    return new ApiResponse<PaymentResponseDTO>(404, "Order not found.");
                }

                if (Math.Round(paymentRequest.Amount, 2) != Math.Round(order.TotalAmount, 2))
                {
                    return new ApiResponse<PaymentResponseDTO>(400, "Payment amount does not match the order total.");
                }

                Payment payment;

                if (order.Payment != null)
                {
                    if (order.Payment.Status == PaymentStatus.Failed && order.OrderStatus == OrderStatus.Pending)
                    {
                        payment = order.Payment;
                        payment.PaymentMethod = paymentRequest.PaymentMethod;
                        payment.Amount = paymentRequest.Amount;
                        payment.PaymentDate = DateTime.UtcNow;
                        payment.Status = PaymentStatus.Pending;
                        payment.TransactionId = null;
                        await _unitOfWork.Payments.UpdatePaymentAsync(payment);
                    }
                    else
                    {
                        return new ApiResponse<PaymentResponseDTO>(400, "Order already has an associated payment.");
                    }
                }
                else
                {
                    payment = new Payment
                    {
                        OrderId = paymentRequest.OrderId,
                        PaymentMethod = paymentRequest.PaymentMethod,
                        Amount = paymentRequest.Amount,
                        PaymentDate = DateTime.UtcNow,
                        Status = PaymentStatus.Pending
                    };

                    await _unitOfWork.Payments.CreatePaymentAsync(payment);
                }

                if (!IsCashOnDelivery(paymentRequest.PaymentMethod))
                {
                    var simulatedStatus = await SimulatePaymentGateway();
                    payment.Status = simulatedStatus;
                    if (simulatedStatus == PaymentStatus.Completed)
                    {
                        payment.TransactionId = GenerateTransactionId();
                        order.OrderStatus = OrderStatus.Processing;
                        await _unitOfWork.Payments.UpdateOrderAsync(order);
                    }
                }
                else
                {
                    order.OrderStatus = OrderStatus.Processing;
                    await _unitOfWork.Payments.UpdateOrderAsync(order);
                }

                await _unitOfWork.Payments.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                if (order.OrderStatus == OrderStatus.Processing)
                {
                    await _emailHelper.SendOrderConfirmationEmailAsync(paymentRequest.OrderId);
                }

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
                await _unitOfWork.RollbackAsync();
                return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while processing the payment.");
            }
        }

        public async Task<ApiResponse<PaymentResponseDTO>> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetPaymentByIdAsync(paymentId);

                if (payment == null)
                {
                    return new ApiResponse<PaymentResponseDTO>(404, "Payment not found.");
                }

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
                return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while retrieving the payment.");
            }
        }

        public async Task<ApiResponse<PaymentResponseDTO>> GetPaymentByOrderIdAsync(int orderId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetPaymentByOrderIdAsync(orderId);

                if (payment == null)
                {
                    return new ApiResponse<PaymentResponseDTO>(404, "Payment not found for this order.");
                }

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
                return new ApiResponse<PaymentResponseDTO>(500, "An unexpected error occurred while retrieving the payment.");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdatePaymentStatusAsync(PaymentStatusUpdateDTO statusUpdate)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetPaymentWithOrderAsync(statusUpdate.PaymentId);

                if (payment == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Payment not found.");
                }

                payment.Status = statusUpdate.Status;
                if (statusUpdate.Status == PaymentStatus.Completed && !IsCashOnDelivery(payment.PaymentMethod))
                {
                    payment.TransactionId = statusUpdate.TransactionId;
                    payment.Order.OrderStatus = OrderStatus.Processing;
                }

                await _unitOfWork.Payments.UpdatePaymentAsync(payment);
                if (payment.Order != null)
                {
                    await _unitOfWork.Payments.UpdateOrderAsync(payment.Order);
                }
                await _unitOfWork.Payments.SaveChangesAsync();

                if (payment.Order?.OrderStatus == OrderStatus.Processing)
                {
                    await _emailHelper.SendOrderConfirmationEmailAsync(payment.Order.Id);
                }

                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"Payment with ID {payment.Id} updated to status '{payment.Status}'."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, "An unexpected error occurred while updating the payment status.");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> CompleteCODPaymentAsync(CODPaymentUpdateDTO codPaymentUpdateDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var payment = await _unitOfWork.Payments.GetPaymentWithOrderByOrderIdAsync(codPaymentUpdateDTO.PaymentId, codPaymentUpdateDTO.OrderId);

                if (payment == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Payment not found.");
                }

                if (payment.Order == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "No Order associated with this Payment.");
                }

                if (payment.Order.OrderStatus != OrderStatus.Shipped)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, $"Order cannot be marked as Delivered from {payment.Order.OrderStatus} State");
                }

                if (!IsCashOnDelivery(payment.PaymentMethod))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(409, "Payment method is not Cash on Delivery.");
                }

                payment.Status = PaymentStatus.Completed;
                payment.Order.OrderStatus = OrderStatus.Delivered;

                await _unitOfWork.Payments.UpdatePaymentAsync(payment);
                await _unitOfWork.Payments.UpdateOrderAsync(payment.Order);
                await _unitOfWork.Payments.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"COD Payment for Order ID {payment.Order.Id} has been marked as 'Completed' and the order status updated to 'Delivered'."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse<ConfirmationResponseDTO>(500, "An unexpected error occurred while completing the COD payment.");
            }
        }

        #region Helper Methods

        private bool IsCashOnDelivery(string paymentMethod)
        {
            return paymentMethod.Equals("CashOnDelivery", StringComparison.OrdinalIgnoreCase) ||
                   paymentMethod.Equals("COD", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<PaymentStatus> SimulatePaymentGateway()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1));

            int chance = Random.Shared.Next(1, 101);
            if (chance <= 60)
                return PaymentStatus.Completed;
            else if (chance <= 90)
                return PaymentStatus.Pending;
            else
                return PaymentStatus.Failed;
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{Guid.NewGuid().ToString("N").ToUpper().Substring(0, 12)}";
        }

        #endregion Helper Methods
    }
}