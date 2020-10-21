using AuthDemo.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthDemo.Web.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult PostLogin([FromBody] PostLoginDto dto)
        {
            if (dto.Username != "test" || dto.Password != "test")
                return BadRequest("Invalid username or password");


            var handler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = DemoDefaults.Audience,
                Issuer = DemoDefaults.Issuer,
                Expires = DateTime.UtcNow.AddDays(1),

                Subject = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, dto.Username)
                }),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Security:SigningKey"))),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
            var jwt = handler.WriteToken(token);

            return Ok(jwt);
        }
    }
}
