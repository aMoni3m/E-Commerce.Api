using E_Commerce.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Api.DTOs.PaymentDTOs
{
    public class PaymentStatusUpdateDTO
    {
        [Required(ErrorMessage = "Payment ID is required.")]
        public int PaymentId { get; set; }

        public string? TransactionId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public PaymentStatus Status { get; set; } // e.g., "Completed", "Failed"
    }
}