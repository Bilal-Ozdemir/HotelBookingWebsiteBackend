using BackEnd.DTOs;
using BackEnd.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _email;

        public ContactController(IEmailService email)
            => _email = email;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ContactDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
            var body = new StringBuilder();
            body.AppendLine($"Name: {dto.Name}");
            body.AppendLine($"Email: {dto.Email}");
            body.AppendLine($"Subject: {dto.Subject}");
            body.AppendLine();
            body.AppendLine(dto.Message);

            try
            {
                
                await _email.SendAsync(
                    fromEmail: dto.Email,
                    fromName:  dto.Name,
                    toEmail:   "hotelcustomerservice123@gmail.com",
                    subject:   dto.Subject,
                    body:      body.ToString()
                );

                return Ok(new { message = "Your message has been sent!" });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new {
                    error  = "An unexpected error occurred.",
                    detail = ex.Message
                });
            }
        }
    }
}
