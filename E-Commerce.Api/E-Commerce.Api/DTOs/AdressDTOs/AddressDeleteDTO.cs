using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Api.DTOs.AdressDTOs
{
    public class AddressDeleteDTO
    {
        [Required(ErrorMessage = "CustomerId is Required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "AddressId is Required")]
        public int AddressId { get; set; }
    }
}