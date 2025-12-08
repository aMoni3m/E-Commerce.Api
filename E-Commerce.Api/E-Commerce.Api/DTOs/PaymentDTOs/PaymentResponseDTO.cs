using E_Commerce.Api.Models;

namespace E_Commerce.Api.DTOs.PaymentDTOs
{
    public class PaymentResponseDTO
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
    }
}