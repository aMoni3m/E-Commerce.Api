using E_Commerce.Api.Data;
using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CancellationDTOs;
using E_Commerce.Api.Helpers;
using E_Commerce.Api.Models;
using E_Commerce.Api.Repository.Interfaces;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Api.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICancellationRepository _cancellationRepository;
        private readonly CancellationEmailHelper _emailHelper;

        public CancellationService(ApplicationDbContext context, ICancellationRepository cancellationRepository, IEmailService emailService)
        {
            _context = context;
            _cancellationRepository = cancellationRepository;
            _emailHelper = new CancellationEmailHelper(emailService);
        }

        public async Task<ApiResponse<CancellationResponseDTO>> RequestCancellationAsync(CancellationRequestDTO cancellationRequest)
        {
            try
            {
                var order = await _cancellationRepository.GetOrderForCancellationAsync(cancellationRequest.OrderId, cancellationRequest.CustomerId);

                if (order == null)
                {
                    return new ApiResponse<CancellationResponseDTO>(404, "Order not found.");
                }

                if (order.OrderStatus != OrderStatus.Processing)
                {
                    return new ApiResponse<CancellationResponseDTO>(400, "Order is not eligible for cancellation.");
                }

                var existingCancellation = await _cancellationRepository.GetCancellationByOrderIdAsync(cancellationRequest.OrderId);

                if (existingCancellation != null)
                {
                    return new ApiResponse<CancellationResponseDTO>(400, "A cancellation request for this order already exists.");
                }

                var cancellation = new Cancellation
                {
                    OrderId = cancellationRequest.OrderId,
                    Reason = cancellationRequest.Reason,
                    Status = CancellationStatus.Pending,
                    RequestedAt = DateTime.UtcNow,
                    OrderAmount = order.TotalAmount,
                    CancellationCharges = 0.00m
                };

                await _cancellationRepository.CreateCancellationAsync(cancellation);
                await _cancellationRepository.SaveChangesAsync();

                var cancellationResponse = new CancellationResponseDTO
                {
                    Id = cancellation.Id,
                    OrderId = cancellation.OrderId,
                    Reason = cancellation.Reason,
                    OrderAmount = order.TotalAmount,
                    Status = cancellation.Status,
                    RequestedAt = cancellation.RequestedAt,
                    CancellationCharges = cancellation.CancellationCharges
                };

                return new ApiResponse<CancellationResponseDTO>(200, cancellationResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CancellationResponseDTO>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CancellationResponseDTO>> GetCancellationByIdAsync(int id)
        {
            try
            {
                var cancellation = await _cancellationRepository.GetCancellationByIdAsync(id);

                if (cancellation == null)
                {
                    return new ApiResponse<CancellationResponseDTO>(404, "Cancellation request not found.");
                }

                var cancellationResponse = new CancellationResponseDTO
                {
                    Id = cancellation.Id,
                    OrderId = cancellation.OrderId,
                    Reason = cancellation.Reason,
                    Status = cancellation.Status,
                    RequestedAt = cancellation.RequestedAt,
                    ProcessedAt = cancellation.ProcessedAt,
                    ProcessedBy = cancellation.ProcessedBy,
                    Remarks = cancellation.Remarks,
                    OrderAmount = cancellation.OrderAmount,
                    CancellationCharges = cancellation.CancellationCharges
                };

                return new ApiResponse<CancellationResponseDTO>(200, cancellationResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CancellationResponseDTO>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateCancellationStatusAsync(CancellationStatusUpdateDTO statusUpdate)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cancellation = await _cancellationRepository.GetCancellationWithOrderAndCustomerAsync(statusUpdate.CancellationId);

                    if (cancellation == null)
                    {
                        return new ApiResponse<ConfirmationResponseDTO>(404, "Cancellation request not found.");
                    }

                    if (cancellation.Status != CancellationStatus.Pending)
                    {
                        return new ApiResponse<ConfirmationResponseDTO>(400, "Only pending cancellation requests can be updated.");
                    }

                    cancellation.Status = statusUpdate.Status;
                    cancellation.ProcessedAt = DateTime.UtcNow;
                    cancellation.ProcessedBy = statusUpdate.ProcessedBy;
                    cancellation.Remarks = statusUpdate.Remarks;

                    if (statusUpdate.Status == CancellationStatus.Approved)
                    {
                        cancellation.Order.OrderStatus = OrderStatus.Canceled;
                        cancellation.CancellationCharges = statusUpdate.CancellationCharges;

                        var orderItems = await _cancellationRepository.GetOrderItemsWithProductByOrderIdAsync(cancellation.OrderId);

                        foreach (var item in orderItems)
                        {
                            item.Product.StockQuantity += item.Quantity;
                            await _cancellationRepository.UpdateProductAsync(item.Product);
                        }
                    }

                    await _cancellationRepository.UpdateCancellationAsync(cancellation);
                    if (cancellation.Order != null)
                    {
                        await _cancellationRepository.UpdateOrderAsync(cancellation.Order);
                    }
                    await _cancellationRepository.SaveChangesAsync();
                    await transaction.CommitAsync();

                    if (statusUpdate.Status == CancellationStatus.Approved)
                    {
                        await _emailHelper.NotifyCancellationAcceptedAsync(cancellation);
                    }
                    else if (statusUpdate.Status == CancellationStatus.Rejected)
                    {
                        await _emailHelper.NotifyCancellationRejectionAsync(cancellation);
                    }

                    var confirmation = new ConfirmationResponseDTO
                    {
                        Message = $"Cancellation request with ID {cancellation.Id} has been {cancellation.Status}."
                    };

                    return new ApiResponse<ConfirmationResponseDTO>(200, confirmation);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred: {ex.Message}");
                }
            }
        }

        public async Task<ApiResponse<List<CancellationResponseDTO>>> GetAllCancellationsAsync()
        {
            try
            {
                var cancellations = await _cancellationRepository.GetAllCancellationsWithOrderAsync();

                var cancellationList = cancellations.Select(c => new CancellationResponseDTO
                {
                    Id = c.Id,
                    OrderId = c.OrderId,
                    Reason = c.Reason,
                    Status = c.Status,
                    RequestedAt = c.RequestedAt,
                    ProcessedAt = c.ProcessedAt,
                    ProcessedBy = c.ProcessedBy,
                    OrderAmount = c.OrderAmount,
                    CancellationCharges = c.CancellationCharges,
                    Remarks = c.Remarks
                }).ToList();

                return new ApiResponse<List<CancellationResponseDTO>>(200, cancellationList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CancellationResponseDTO>>(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
