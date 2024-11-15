﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTAuthenticationForProduct.Models;
using System.Net;
using webapic_.Services;
using System.Collections.Concurrent;



namespace JWTAuthenticationForProduct.Controllers
{

    [Route("api/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IJwtBlacklistService _jwtBlacklistService;

        public LoginController(IConfiguration config, IJwtBlacklistService jwtBlacklistService)
        {

            _config = config;
            _jwtBlacklistService = jwtBlacklistService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserModel login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private UserModel AuthenticateUser(UserModel login)
        {

            UserModel user = null;

            if (login.Username.ToLower() == "string")

            {
                user = new UserModel { Username = login.Username, EmailAddress = login.EmailAddress, DateOfJoing = login.DateOfJoing };
            };
            return user;
        }

        private string GenerateJSONWebToken(UserModel userInfo)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var cardentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress),
                new Claim("DateOfJoining",userInfo.DateOfJoing.ToString("yyyy-mm-dd")),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };


            var token = new JwtSecurityToken(
          issuer: _config["Jwt:Issuer"],
          audience: _config["Jwt:Issuer"],
          claims: claims,
          expires: DateTime.Now.AddMinutes(30),
          signingCredentials: cardentials
      );

            return new JwtSecurityTokenHandler().WriteToken(token);



        }

        private static ConcurrentDictionary<string, bool> _tokenBlacklist = new();


        [Authorize]
        [HttpPost]

        public IActionResult Logout()
        {
            /*var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                _tokenBlacklist.TryAdd(token, true);
                return Ok(new { message = "User logged out successfully." });
            }
            return BadRequest(new { message = "Invalid token format." });*/
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                _jwtBlacklistService.BlacklistToken(token);
            }

            return Ok("Logged out successfully");
        }
      
        /*private bool IsTokenBlacklisted(string token)
        {
            return _tokenBlacklist.ContainsKey(token);
        }

        // Middleware to check if the token is blacklisted
        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (IsTokenBlacklisted(token))
            {
                return Unauthorized(new { message = "Token has been revoked. Please log in again." });
            }

            return Ok(new { message = "This is a protected endpoint." });
        }*/
    }

}


