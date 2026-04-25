using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string login, string password);
        Task<string> RegisterAsync(RegisterDto dto);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
