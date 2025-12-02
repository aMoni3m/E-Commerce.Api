using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Api.DTOs.ProductDTOs
{
    public class ProductStatusUpdateDTO
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "IsAvailable is required.")]
        public bool IsAvailable { get; set; }
    }
}