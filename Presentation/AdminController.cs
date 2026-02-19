using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstraction;
using Shared.Dtos;
using Shared.Dtos.Admin;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AdminController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AdminAuthResultDto>> Login([FromBody] AdminLoginDto loginDto)
        {
            var result = await _serviceManager.AdminService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        [Authorize(Roles = "SuperAdmin")] 
        public async Task<ActionResult<AdminAuthResultDto>> Register([FromBody] AdminCreateDto createDto)
        {
            var result = await _serviceManager.AdminService.RegisterAsync(createDto);
            return Ok(result);
        }


        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<AdminAuthResultDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _serviceManager.AdminService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            return Ok(result);
        }
        [HttpGet("admins")]
        [Authorize(Roles = "SuperAdmin")]  // ✅ فقط Super Admin
        public async Task<ActionResult<IEnumerable<AdminResultDto>>> GetAllAdmins()
        {
            var result = await _serviceManager.AdminService.GetAllAdminsAsync();
            return Ok(result);
        }
        [HttpGet("admins/{id}")]
        [Authorize(Roles = "SuperAdmin")]  // ✅ فقط SuperAdmin
        public async Task<ActionResult<AdminResultDto>> GetAdminById(Guid id)
        {
            var result = await _serviceManager.AdminService.GetAdminByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]  // ✅ أي Admin مسجل دخول يقدر يعمل Logout
        public async Task<ActionResult> Logout()
        {
            var adminId = GetAdminIdFromClaims();
            await _serviceManager.AdminService.LogoutAsync(adminId);
            return Ok(new { message = "Logged out successfully" });
        }

        private Guid GetAdminIdFromClaims()
        {
            var adminIdClaim = User.FindFirst("admin_id")?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !Guid.TryParse(adminIdClaim, out var adminId))
            {
                throw new UnauthorizedAccessException("Invalid admin ID in token");
            }
            return adminId;
        }
    }
}