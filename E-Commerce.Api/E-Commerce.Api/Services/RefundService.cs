using E_Commerce.Api.Data;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.RefundDTOs;
using E_Commerce.Api.Helpers;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Services
{
    public class RefundService : IRefundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly RefundEmailHelper _emailHelper;

        public RefundService(ApplicationDbContext context, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _context = context;
            _unitOfWork = unitOfWork;

            _emailHelper = new RefundEmailHelper(emailService);
        }

        public async Task<ApiResponse<List<PendingRefundResponseDTO>>> GetEligibleRefundsAsync()
        {
            try
            {
                var eligible = await _unitOfWork.Refunds.GetEligibleCancellationsForRefundAsync();

                var result = eligible.Select(c => new PendingRefundResponseDTO
                {
                    CancellationId = c.Id,
                    OrderId = c.OrderId,
                    OrderAmount = c.OrderAmount,
                    CancellationCharge = c.CancellationCharges ?? 0.00m,
                    ComputedRefundAmount = c.OrderAmount - (c.CancellationCharges ?? 0.00m),
                    CancellationRemarks = c.Remarks
                }).ToList();

                return new ApiResponse<List<PendingRefundResponseDTO>>(200, result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<PendingRefundResponseDTO>>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RefundResponseDTO>> ProcessRefundAsync(RefundRequestDTO refundRequest)
        {
            try
            {
                var cancellation = await _unitOfWork.Refunds.GetCancellationWithOrderPaymentAndCustomerAsync(refundRequest.CancellationId);

                if (cancellation == null)
                {
                    return new ApiResponse<RefundResponseDTO>(404, "Cancellation request not found.");
                }

                if (cancellation.Status != CancellationStatus.Approved)
                {
                    return new ApiResponse<RefundResponseDTO>(400, "Only approved cancellations are eligible for refunds.");
                }

                var existingRefund = await _unitOfWork.Refunds.GetRefundByCancellationIdAsync(refundRequest.CancellationId);

                if (existingRefund != null)
                {
                    return new ApiResponse<RefundResponseDTO>(400, "Refund for this cancellation request has already been initiated.");
                }

                var payment = cancellation.Order.Payment;

                if (payment == null || payment.PaymentMethod.ToLower() == "cod")
                {
                    return new ApiResponse<RefundResponseDTO>(400, "No payment associated with the order.");
                }

                decimal computedRefundAmount = cancellation.OrderAmount - (cancellation.CancellationCharges ?? 0.00m);

                if (computedRefundAmount <= 0)
                {
                    return new ApiResponse<RefundResponseDTO>(400, "Computed refund amount is not valid.");
                }

                var refund = new Refund
                {
                    CancellationId = refundRequest.CancellationId,
                    PaymentId = payment.Id,
                    Amount = computedRefundAmount,
                    RefundMethod = refundRequest.RefundMethod.ToString(),
                    RefundReason = refundRequest.RefundReason,
                    Status = RefundStatus.Pending,
                    InitiatedAt = DateTime.UtcNow,
                    ProcessedBy = refundRequest.ProcessedBy
                };

                await _unitOfWork.Refunds.CreateRefundAsync(refund);
                await _unitOfWork.SaveChangesAsync();

                var gatewayResponse = await ProcessRefundPaymentAsync(refund);

                if (gatewayResponse.IsSuccess)
                {
                    refund.Status = RefundStatus.Completed;
                    refund.TransactionId = gatewayResponse.TransactionId;
                    refund.CompletedAt = DateTime.UtcNow;
                    payment.Status = PaymentStatus.Refunded;

                    _unitOfWork.Refunds.UpdateRefundAsync(refund);
                    _unitOfWork.Refunds.UpdatePaymentAsync(payment);
                    await _unitOfWork.SaveChangesAsync();

                    if (cancellation.Order.Customer != null && !string.IsNullOrEmpty(cancellation.Order.Customer.Email))
                    {
                        await _emailHelper.SendRefundSuccessEmailAsync(
                            refund,
                            cancellation.Order.OrderNumber,
                            cancellation,
                            cancellation.Order.Customer.Email);
                    }
                }
                else
                {
                    refund.Status = RefundStatus.Failed;
                    _unitOfWork.Refunds.UpdateRefundAsync(refund);
                    await _unitOfWork.SaveChangesAsync();
                }

                return new ApiResponse<RefundResponseDTO>(200, MapRefundToDTO(refund));
            }
            catch (Exception ex)
            {
                return new ApiResponse<RefundResponseDTO>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateRefundStatusAsync(RefundStatusUpdateDTO statusUpdate)
        {
            try
            {
                var refund = await _unitOfWork.Refunds.GetRefundWithDetailsAsync(statusUpdate.RefundId);

                if (refund == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Refund not found.");
                }

                if (refund.Status != RefundStatus.Pending && refund.Status != RefundStatus.Failed)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Only pending or failed refunds can be updated.");
                }

                refund.RefundMethod = statusUpdate.RefundMethod.ToString();
                refund.Status = RefundStatus.Completed;
                refund.TransactionId = statusUpdate.TransactionId;
                refund.CompletedAt = DateTime.UtcNow;
                refund.ProcessedBy = statusUpdate.ProcessedBy;
                refund.RefundReason = statusUpdate.RefundReason;

                refund.Payment.Status = PaymentStatus.Refunded;

                _unitOfWork.Refunds.UpdateRefundAsync(refund);
                _unitOfWork.Refunds.UpdatePaymentAsync(refund.Payment);
                await _unitOfWork.SaveChangesAsync();

                if (refund.Cancellation?.Order?.Customer != null && !string.IsNullOrEmpty(refund.Cancellation.Order.Customer.Email))
                {
                    await _emailHelper.SendRefundSuccessEmailAsync(
                        refund,
                        refund.Cancellation.Order.OrderNumber,
                        refund.Cancellation,
                        refund.Cancellation.Order.Customer.Email);
                }

                var confirmation = new ConfirmationResponseDTO
                {
                    Message = $"Refund with ID {refund.Id} has been updated to {refund.Status}."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RefundResponseDTO>> GetRefundByIdAsync(int id)
        {
            try
            {
                var refund = await _unitOfWork.Refunds.GetRefundByIdAsync(id);

                if (refund == null)
                {
                    return new ApiResponse<RefundResponseDTO>(404, "Refund not found.");
                }

                return new ApiResponse<RefundResponseDTO>(200, MapRefundToDTO(refund));
            }
            catch (Exception ex)
            {
                return new ApiResponse<RefundResponseDTO>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<RefundResponseDTO>>> GetAllRefundsAsync()
        {
            try
            {
                var refunds = await _unitOfWork.Refunds.GetAllRefundsAsync();

                var refundList = refunds.Select(r => MapRefundToDTO(r)).ToList();

                return new ApiResponse<List<RefundResponseDTO>>(200, refundList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RefundResponseDTO>>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        #region Helper Methods

        private RefundResponseDTO MapRefundToDTO(Refund refund)
        {
            return new RefundResponseDTO
            {
                Id = refund.Id,
                CancellationId = refund.CancellationId,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                RefundMethod = Enum.Parse<RefundMethod>(refund.RefundMethod),
                RefundReason = refund.RefundReason,
                Status = refund.Status,
                InitiatedAt = refund.InitiatedAt,
                CompletedAt = refund.CompletedAt,
                TransactionId = refund.TransactionId
            };
        }

        private async Task<PaymentGatewayRefundResponseDTO> ProcessRefundPaymentAsync(Refund refund)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            var random = new Random();
            double chance = random.NextDouble();

            if (chance < 0.70)
            {
                return new PaymentGatewayRefundResponseDTO
                {
                    IsSuccess = true,
                    Status = RefundStatus.Completed,
                    TransactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
                };
            }
            else if (chance < 0.90)
            {
                return new PaymentGatewayRefundResponseDTO
                {
                    IsSuccess = false,
                    Status = RefundStatus.Failed
                };
            }
            else
            {
                return new PaymentGatewayRefundResponseDTO
                {
                    IsSuccess = false,
                    Status = RefundStatus.Pending
                };
            }
        }

        #endregion Helper Methods
    }
}