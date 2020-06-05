using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Wyklad3.Models;
using Wyklad3.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using Wyklad3.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace Wyklad3.Controllers
{
    [Route("auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        private IDbService _dbService;

        public LoginController(IDbService service, IConfiguration configuration)
        {
            _dbService = service;
            Configuration = configuration;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginRequest request)
        {
            //if (!_dbService.AreCredentialsValid(request, Encoding.UTF8.GetBytes(Configuration["SecretKey"])))
            //{
            //    return Unauthorized("wrong credentials");
            //}

            LoginResponse resp = generateTokens(request.Login);

            _dbService.SetNewRefreshToken(request.Login, resp.RefreshToken);

            return Ok(resp);
        }
        
        [HttpPost("refreshToken")]
        public IActionResult RefreshToken([FromBody]RefreshTokenRequest request)
        {

            Student user;
            try
            {
                user = _dbService.GetStudentByRefreshToken(request.RefreshToken);
            } catch (StudentNotFoundException)
            {
                return Unauthorized("wrong refresh token");
            }

            LoginResponse resp = generateTokens(user.IndexNumber);

            _dbService.SetNewRefreshToken(user.IndexNumber, resp.RefreshToken);

            return Ok(resp);
        }

        private LoginResponse generateTokens(String login)
        {
            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, login),
                new Claim(ClaimTypes.Role, "student"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds
            );

            LoginResponse resp = new LoginResponse();
            resp.Token = new JwtSecurityTokenHandler().WriteToken(token);
            resp.RefreshToken = Guid.NewGuid().ToString();

            return resp;
        }
    }
}