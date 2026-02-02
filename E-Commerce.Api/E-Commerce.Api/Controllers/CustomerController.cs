using E_Commerce.Api.DTOs;
using E_Commerce.Api.DTOs.CustomerDTOs;
using E_Commerce.Api.Services;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerRegistration([FromBody] CustomerRegistrationDTO customerRegistration)
        {
            var response = await _customerService.RegisterCustomer(customerRegistration);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById([FromRoute] int id)
        {
            var response = await _customerService.FindCustomer(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerUpdateDTO customerUpdate)
        {
            var response = await _customerService.UpdateCustomer(customerUpdate);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("pageSize/{pageSize}/pageNumber/{pageNumber}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AllCustomer(int pageSize = 10, int pageNumber = 1)
        {
            var response = await _customerService.AllCustomer(pageSize, pageNumber);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            var response = await _customerService.DeleteCustomer(id);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}