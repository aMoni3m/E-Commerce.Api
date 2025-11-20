using E_Commerce.Api.DTOs.AdressDTOs;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> AllAddresses()
        {
            var response = await _addressService.AllAddressesAsync();

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressCreateDTO addressCreateDTO)
        {
            var response = await _addressService.CreateAddressAsync(addressCreateDTO);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressUpdateDTO addressUpdateDTO)
        {
            var response = await _addressService.UpdateAddressAsync(addressUpdateDTO);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAddress([FromBody] AddressDeleteDTO addressDeleteDTO)
        {
            var response = await _addressService.DeleteAddressAsync(addressDeleteDTO);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}