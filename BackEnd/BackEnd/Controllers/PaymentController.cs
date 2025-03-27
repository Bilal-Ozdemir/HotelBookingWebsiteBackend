using BackEnd.Entities;
using BackEnd.UseCases.Payments; // Ensure this namespace matches your project structure
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly GetPayments _getPayments;
        private readonly GetPayment _getPayment;
        private readonly CreatePayment _createPayment;
        private readonly UpdatePayment _updatePayment;
        private readonly DeletePayment _deletePayment;

        public PaymentController(
            GetPayments getPayments,
            GetPayment getPayment,
            CreatePayment createPayment,
            UpdatePayment updatePayment,
            DeletePayment deletePayment)
        {
            _getPayments = getPayments;
            _getPayment = getPayment;
            _createPayment = createPayment;
            _updatePayment = updatePayment;
            _deletePayment = deletePayment;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            var payments = await _getPayments.Execute();
            return Ok(payments);
        }

        // GET: api/payment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _getPayment.Execute(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment([FromBody] Payment payment)
        {
            try
            {
                var createdPayment = await _createPayment.Execute(payment);
                return CreatedAtAction(nameof(GetPayment), new { id = createdPayment.Id }, createdPayment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/payment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment updatedPayment)
        {
            try
            {
                await _updatePayment.Execute(id, updatedPayment);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/payment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                await _deletePayment.Execute(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}