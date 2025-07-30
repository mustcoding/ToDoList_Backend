using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Application;
using ToDoList.Application.DTOs;
using ToDoList.Core.Entities;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ToDoList.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IConfiguration _config;

    public UserController(IUserService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [HttpGet]
    public ActionResult GetAll() => Ok(_service.GetAll());

    [HttpGet("{ulid}")]
    public ActionResult Get(string ulid)
    {
        var user = _service.Get(ulid);
        return user == null ? NotFound() : Ok(user);
    }


    [HttpPutAttribute("{ulid}")]
    public ActionResult Update(string ulid, [FromBody] User user)
    {
        if (ulid != user.Ulid) return BadRequest("Ulid mismatch.");
        _service.Update(user);
        return NoContent();
    }

    [HttpDelete("{ulid}")]
    public ActionResult Delete(string ulid)
    {
        _service.Delete(ulid);
        return NoContent();
    }

    [HttpPost("register")]
    public ActionResult Register([FromBody] UserRegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            return BadRequest("Username is required.");
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Email is required.");
        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Password is required.");

        try
        {
            var user = _service.Register(dto);
            return CreatedAtAction(nameof(Get), new { ulid = user.Ulid }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] UserRegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Please enter email");
        if (string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Please enter password");
        try
        {
            var user = _service.Login(dto);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
           {
                new Claim(ClaimTypes.NameIdentifier, user.Ulid),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
               new SymmetricSecurityKey(key),
               SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    user.UserId,
                    user.Ulid,
                    user.Username,
                    user.Email
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { measage = ex.Message });
        }
    }

}