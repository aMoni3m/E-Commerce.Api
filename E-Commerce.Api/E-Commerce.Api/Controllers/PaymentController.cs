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

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById([FromRoute] int paymentId)
        {
            var response = await _paymentService.GetPaymentByIdAsync(paymentId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId([FromRoute] int orderId)
        {
            var response = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentStatusUpdateDTO statusUpdate)
        {
            var response = await _paymentService.UpdatePaymentStatusAsync(statusUpdate);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("cod/complete")]
        public async Task<IActionResult> CompleteCODPayment([FromBody] CODPaymentUpdateDTO codPaymentUpdateDTO)
        {
            var response = await _paymentService.CompleteCODPaymentAsync(codPaymentUpdateDTO);
            return StatusCode(response.StatusCode, response);
        }
    }
}
