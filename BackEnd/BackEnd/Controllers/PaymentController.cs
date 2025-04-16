// BackEnd/Controllers/PaymentController.cs
using BackEnd.DTOs;
using BackEnd.Entities;
using BackEnd.UseCases.Payments;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaymentController : ControllerBase
    {
        private readonly CreatePayment _createPayment;

        public PaymentController(CreatePayment createPayment)
        {
            _createPayment = createPayment;
        }

        // POST: api/payment
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentRequestDto request)
        {
            // parse user ID from token
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim.Value, out var userId))
                return Unauthorized(new { error = "Invalid or missing token." });

            // Map DTO → entity
            var paymentEntity = new Payment
            {
                BookingId     = request.BookingId,
                UserId        = userId,
                Amount        = request.Amount,
                PaymentDate   = DateTime.UtcNow,
                PaymentMethod = "SIMULATED"
            };

            try
            {
                var created = await _createPayment.Execute(paymentEntity);

                var dto = new PaymentDto
                {
                    Id          = created.Id,
                    BookingId   = created.BookingId,
                    Amount      = created.Amount,
                    PaymentDate = created.PaymentDate
                };

                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                // Known bad input (e.g. booking not found)
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Anything else becomes a JSON 500
                return StatusCode(500, new 
                { 
                    error   = "Internal server error",
                    detail  = ex.Message
                });
            }
        }

        // GET: api/payment/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromServices] GetPayment getPayment)
        {
            var p = await getPayment.Execute(id);
            if (p == null) return NotFound(new { error = "Payment not found." });

            var dto = new PaymentDto
            {
                Id          = p.Id,
                BookingId   = p.BookingId,
                Amount      = p.Amount,
                PaymentDate = p.PaymentDate
            };
            return Ok(dto);
        }
    }
}
