using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Entities;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context,ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await IsUsernameExists(registerDto.Username)) return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();
            var appUser = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(appUser);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                UserName = registerDto.Username,
                Token = _tokenService.CreateToken(appUser)
            };
        }

        private async Task<bool> IsUsernameExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName.Equals(username.ToLower()));
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var appUser = await _context.Users
                .SingleOrDefaultAsync(x=> x.UserName.Equals(loginDto.Username));

            if (appUser == null) return Unauthorized("invalid username");

            using var hmac = new HMACSHA512(appUser.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != appUser.PasswordHash[i]) return Unauthorized("password invalid");
            }

            return new UserDto
            {
                UserName = appUser.UserName,
                Token = _tokenService.CreateToken(appUser)
            };
        }
    }
}
