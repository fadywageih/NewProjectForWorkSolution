using Shared.Dtos;
using Shared.Dtos.User;

namespace ServicesAbstraction
{
    public interface IAuthenticationService
    {
        public Task<UserResultDto> Login(LoginDto loginDto);
        Task<UserResultDto> RegisterUser(UserRegisterDto dto);
        public Task<UserResultDto> GetUserByEmail(string email);
        public Task<bool> CheckIfEmailExist(string email);
        public Task<bool> SendResetPasswordEmail(string email);
        public Task<bool> ResetPassword(string email, string token, string password);
    }
}
