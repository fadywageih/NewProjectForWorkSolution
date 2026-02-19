using Shared.Dtos;
using Shared.Dtos.Admin;

namespace ServicesAbstraction
{
    public interface IAdminService
    {
        Task<AdminAuthResultDto> LoginAsync(AdminLoginDto loginDto);
        Task<AdminAuthResultDto> RegisterAsync(AdminCreateDto createDto);
        Task<AdminAuthResultDto> RefreshTokenAsync(string refreshToken);
        Task<AdminResultDto> GetAdminByIdAsync(Guid id);
        Task<IEnumerable<AdminResultDto>> GetAllAdminsAsync();
        Task<bool> LogoutAsync(Guid adminId);
    }
}
