using E_Commerce.Api.DTOs.RefundDTOs;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpGet("eligible")]
        public async Task<IActionResult> GetEligibleRefunds()
        {
            var response = await _refundService.GetEligibleRefundsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessRefund([FromBody] RefundRequestDTO refundRequest)
        {
            var response = await _refundService.ProcessRefundAsync(refundRequest);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateRefundStatus([FromBody] RefundStatusUpdateDTO statusUpdate)
        {
            var response = await _refundService.UpdateRefundStatusAsync(statusUpdate);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRefundById([FromRoute] int id)
        {
            var response = await _refundService.GetRefundByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRefunds()
        {
            var response = await _refundService.GetAllRefundsAsync();
            return StatusCode(response.StatusCode, response);
        }
    }
}
