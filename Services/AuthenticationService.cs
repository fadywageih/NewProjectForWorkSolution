using AutoMapper;
using Domain.Contracts;
using Domain.Entities.User;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServicesAbstraction;
using Shared;
using Shared.Dtos;
using Shared.Dtos.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IOptions<JwtOptions> _options;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IOptions<JwtOptions> options,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _options = options;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<bool> CheckIfEmailExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<UserResultDto> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new UserNotFoundException(email);

            return new UserResultDto(
                DisplayName: user.Name ?? user.Email,
                Email: user.Email,
                Token: await CreateTokenAsync(user)
            );
        }

        public async Task<UserResultDto> RegisterUser(UserRegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                throw new ValidationException(
                    new[] { "Email is already registered." }
                );
            }

            if (await _userManager.FindByNameAsync(dto.Email) != null)
            {
                throw new ValidationException(
                    new[] { "Username/Email is already taken." }
                );
            }

            var user = _mapper.Map<ApplicationUser>(dto);
            user.Id = Guid.NewGuid();

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                throw new ValidationException(errors);
            }

            _logger.LogInformation("User registered successfully: {Email}", user.Email);

            return new UserResultDto(
                DisplayName: user.Name ?? user.Email,
                Email: user.Email,
                Token: await CreateTokenAsync(user)
            );
        }

        public async Task<UserResultDto> Login(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                _logger.LogWarning("Login failed - invalid password for: {Email}", loginDto.Email);
                throw new UnauthorizedException("Invalid email or password");
            }

            _logger.LogInformation("Login successful for: {Email}", loginDto.Email);

            return new UserResultDto(
                DisplayName: user.Name ?? user.Email,
                Email: user.Email,
                Token: await CreateTokenAsync(user)
            );
        }

        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var jwtOptions = _options.Value;
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim("user_id", user.Id.ToString()),
                new Claim("display_name", user.Name ?? user.Email),
                new Claim("address", user.Address ?? string.Empty),
                new Claim("phone", user.PhoneNumber ?? string.Empty),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtOptions.ExpirationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> SendResetPasswordEmail(string email)
        {
            _logger.LogInformation("Password reset requested for email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation("Password reset email sent (or not sent) for security reasons: {Email}", email);
                return true;
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Properly URL-encode the token to preserve special characters like + and /
                var encodedToken = Uri.EscapeDataString(token);
                var resetLink = $"{GetFrontendBaseUrl()}/reset-password?email={Uri.EscapeDataString(email)}&token={encodedToken}";

                _logger.LogInformation("Reset link generated for {Email}", email);

                var emailMessage = new EmailDto
                {
                    To = user.Email,
                    Subject = "Reset Your Password - FM Promotions",
                    Body = BuildResetPasswordEmailBody(user.Name, resetLink)
                };

                await _emailService.SendEmailAsync(emailMessage);

                _logger.LogInformation("Password reset email sent successfully to: {Email}", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reset password email to: {Email}", email);
                throw new Exception("Failed to send reset password email. Please try again later.", ex);
            }
        }
        public async Task<bool> ResetPassword(string email, string token, string password)
        {
            _logger.LogInformation("Password reset attempt for email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Password reset failed - user not found: {Email}", email);
                return false;
            }

            try
            {
                // Note: Angular's ActivatedRoute.queryParams automatically URL-decodes parameters
                // so the token we receive here is already properly decoded.
                // We can use it directly with UserManager.ResetPasswordAsync()
                _logger.LogInformation("Using token: {FirstChars}...",
                    token.Length > 50 ? token.Substring(0, 50) : token);
                _logger.LogInformation("Token length: {Length}", token.Length);

                var result = await _userManager.ResetPasswordAsync(user, token, password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successful for: {Email}", email);
                    return true;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password reset failed for {Email}. Errors: {Errors}", email, errors);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for: {Email}", email);
                return false;
            }
        }
        #region Private Helper Methods

        private string GetFrontendBaseUrl()
        {
            // You can get this from configuration
            return "http://localhost:4200";
        }
        private string BuildResetPasswordEmailBody(string? userName, string resetLink)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background: linear-gradient(to right, #0f204b, #1e3a8a); padding: 20px; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>FM PROMOTIONS</h1>
                        <p style='color: rgba(255,255,255,0.8); margin: 5px 0 0 0;'>Branded Merchandise Specialist</p>
                    </div>
                    
                    <div style='padding: 30px; background-color: #f9fafb;'>
                        <h2 style='color: #0f204b;'>Password Reset Request</h2>
                        
                        <p>Hello {(string.IsNullOrEmpty(userName) ? "there" : userName)},</p>
                        
                        <p>You recently requested to reset your password for your FM Promotions account. 
                           Click the button below to reset it:</p>
                        
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{resetLink}' 
                               style='background-color: #0f204b; color: white; padding: 12px 24px; 
                                      text-decoration: none; border-radius: 6px; font-weight: bold;
                                      display: inline-block;'>
                                Reset Your Password
                            </a>
                        </div>
                        
                        <p>Or copy and paste this link into your browser:</p>
                        <p style='background-color: #e5e7eb; padding: 10px; border-radius: 4px; 
                                   word-break: break-all; font-size: 12px;'>
                            {resetLink}
                        </p>
                        
                        <div style='background-color: #fef3c7; border-left: 4px solid #d97706; 
                                    padding: 12px; margin: 20px 0;'>
                            <p style='margin: 0; color: #92400e;'>
                                <strong>Important:</strong> This password reset link will expire in 24 hours.
                            </p>
                        </div>
                        
                        <p>If you didn't request a password reset, please ignore this email or contact support 
                           if you have concerns about your account's security.</p>
                        
                        <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;'>
                        
                        <p style='color: #6b7280; font-size: 12px;'>
                            This is an automated message, please do not reply to this email.<br>
                            If you need assistance, contact our support team.
                        </p>
                    </div>
                    
                    <div style='background-color: #0f204b; color: white; padding: 15px; text-align: center; font-size: 12px;'>
                        <p style='margin: 0;'>© {DateTime.Now.Year} FM Promotions. All rights reserved.</p>
                    </div>
                </div>
            ";
        }
        #endregion
    }
}