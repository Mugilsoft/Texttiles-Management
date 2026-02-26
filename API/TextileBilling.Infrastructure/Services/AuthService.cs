using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Auth;
using TextileBilling.Core.Entities;
using TextileBilling.Core.Interfaces;

namespace TextileBilling.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            System.Console.WriteLine($"[AuthService] Login attempt for: {loginDto.Email}");
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                System.Console.WriteLine($"[AuthService] User not found: {loginDto.Email}");
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password" };
            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                System.Console.WriteLine($"[AuthService] Password mismatch for: {loginDto.Email}");
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid email or password" };
            }

            System.Console.WriteLine($"[AuthService] Login successful for: {loginDto.Email}");

            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                UserName = user.Email,
                Role = roles.FirstOrDefault(),
                BranchId = user.BranchId
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "User already exists" };
            }

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                BranchId = registerDto.BranchId,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
            }

            if (!await _roleManager.RoleExistsAsync(registerDto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(registerDto.Role));
            }

            await _userManager.AddToRoleAsync(user, registerDto.Role);

            return new AuthResponseDto { IsSuccess = true, Message = "User registered successfully" };
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("branchId", user.BranchId?.ToString() ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
