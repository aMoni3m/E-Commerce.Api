using E_Commerce.Api.DTOs.CancellationDTOs;
using E_Commerce.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly ICancellationService _cancellationService;

        public CancellationController(ICancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }

        [HttpPost]
        public async Task<IActionResult> RequestCancellation([FromBody] CancellationRequestDTO cancellationRequest)
        {
            var response = await _cancellationService.RequestCancellationAsync(cancellationRequest);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCancellationById([FromRoute] int id)
        {
            var response = await _cancellationService.GetCancellationByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCancellations()
        {
            var response = await _cancellationService.GetAllCancellationsAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateCancellationStatus([FromBody] CancellationStatusUpdateDTO statusUpdate)
        {
            var response = await _cancellationService.UpdateCancellationStatusAsync(statusUpdate);
            return StatusCode(response.StatusCode, response);
        }
    }
}