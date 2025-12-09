using E_Commerce.Api.DTOs.PaymentDTOs;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDTO paymentRequest)
        {
            var response = await _paymentService.ProcessPaymentAsync(paymentRequest);
            return StatusCode(response.StatusCode, response);
        }
    }
}
