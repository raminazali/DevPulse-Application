using DevPulse.Application.Common;
using DevPulse.Application.DTOs.Projects;
using DevPulse.Application.Services.Projects.Inetfaces;
using DevPulse.Application.Specifications.Users;
using DevPulse.Core.Entities.Projects;
using DevPulse.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DevPulse.Core.Exceptions;

namespace DevPulse.Application.Services.Projects
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            IRepository<User> userRepo,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var spec = new UserByEmailSpec(request.Email);
            var user = await _userRepo.FirstOrDefaultAsync(spec);

            if (user is null)
                throw new UnauthorizedException(ErrorMessages.UserNotFound, "AUTH_USER_NOT_FOUND");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedException(ErrorMessages.InvalidPassword, "AUTH_INVALID_PASSWORD");

            user.RecordLogin();
            await _userRepo.UpdateAsync(user);

            var token = GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                Token = token,
                ExpiresIn = DateTime.UtcNow.AddDays(1),
                User = new UserDetailDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsAdmin = user.IsAdmin,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    
                }
            };

        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = jwtSettings.GetValue<string>("Key");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
             };

            var keyBytes = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}