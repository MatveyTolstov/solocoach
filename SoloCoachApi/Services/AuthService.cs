using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace SoloCoachApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationContext _context;
        private readonly JwtService _jwtService;
        private readonly ILoggingService _loggingService;
        private readonly IEmailService _emailService;

        public AuthService(
            ApplicationContext context,
            JwtService jwtService,
            ILoggingService loggingService,
            IEmailService emailService)
        {
            _context = context;
            _jwtService = jwtService;
            _loggingService = loggingService;
            _emailService = emailService;
        }

        public async Task<string> LoginAsync(string login, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Login == login);

            if (user == null)
            {
                await _loggingService.LogActionAsync(
                    userId: null,
                    action: "LOGIN_FAILED",
                    entityType: "User",
                    entityId: null,
                    details: new { login, reason = "User not found" },
                    status: "ERROR",
                    errorMessage: "User not found"
                );
                throw new UnauthorizedAccessException("Invalid login or password");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                await _loggingService.LogActionAsync(
                    userId: user.IdUser,
                    action: "LOGIN_FAILED",
                    entityType: "User",
                    entityId: user.IdUser,
                    details: new { login, reason = "Password not set" },
                    status: "ERROR",
                    errorMessage: "Пароль не установлен"
                );
                throw new ArgumentException("Пароль пользователя не установлен. Пожалуйста, свяжитесь с администратором.");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                await _loggingService.LogActionAsync(
                    userId: user.IdUser,
                    action: "LOGIN_FAILED",
                    entityType: "User",
                    entityId: user.IdUser,
                    details: new { login, reason = "Invalid password" },
                    status: "ERROR",
                    errorMessage: "Invalid password"
                );
                throw new UnauthorizedAccessException("Неверный логин или пароль");
            }

            await _loggingService.LogActionAsync(
                userId: user.IdUser,
                action: "LOGIN",
                entityType: "User",
                entityId: user.IdUser,
                details: new { login, roleId = user.RoleId, timestamp = DateTime.UtcNow }
            );

            return _jwtService.GenerateToken(user);
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.Login == dto.Login))
            {
                await _loggingService.LogActionAsync(
                    userId: null,
                    action: "REGISTER_FAILED",
                    entityType: "User",
                    entityId: null,
                    details: new { login = dto.Login, reason = "Логин уже существует" },
                    status: "ERROR",
                    errorMessage: "Логин уже существует"
                );
                throw new ArgumentException("Логин уже существует");
            }

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            {
                await _loggingService.LogActionAsync(
                    userId: null,
                    action: "REGISTER_FAILED",
                    entityType: "User",
                    entityId: null,
                    details: new { email = dto.Email, reason = "Email already exists" },
                    status: "ERROR",
                    errorMessage: "Email already exists"
                );
                throw new ArgumentException("Email уже существует");
            }

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                throw new ArgumentException("Пароль должен состоять как минимум из 6 символов");

            var user = new User
            {
                Name = dto.Name,
                Login = dto.Login,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = 2
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _loggingService.LogActionAsync(
                userId: user.IdUser,
                action: "REGISTER",
                entityType: "User",
                entityId: user.IdUser,
                details: new { login = user.Login, email = user.Email }
            );

            return _jwtService.GenerateToken(user);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null) return; 

            var oldTokens = _context.PasswordResetTokens
                .Where(t => t.UserId == user.IdUser && !t.IsUsed);
            _context.PasswordResetTokens.RemoveRange(oldTokens);

            var code = GenerateSixDigitCode();
            var codeHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(code))
            );

            _context.PasswordResetTokens.Add(new PasswordResetToken
            {
                UserId = user.IdUser,
                TokenHash = codeHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            });
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Восстановление пароля — SoloCoach",
                PasswordResetTemplate(code)
            );
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var email = dto.Email?.Trim();
            var code = dto.Code?.Trim();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Email и код сброса обязательны для заполнения.");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
                throw new ArgumentException("Недействительный или просроченный код сброса.");

            if(BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.Password))
                throw new ArgumentException("Новый пароль не может совпадать с предыдущим.");

            var incomingCodeHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(code))
            );

            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.UserId == user.IdUser &&
                    t.TokenHash == incomingCodeHash &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow) ?? throw new ArgumentException("Недействительный или просроченный код сброса");

            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
                throw new ArgumentException("Пароль должен состоять как минимум из 6 символов");

            resetToken.User.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            resetToken.IsUsed = true;

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                resetToken.User.Email,
                "Пароль изменён — SoloCoach",
                "Ваш пароль был успешно изменён. Если это были не вы — срочно свяжитесь с поддержкой."
            );

            await _loggingService.LogActionAsync(
                userId: resetToken.UserId,
                action: "PASSWORD_RESET",
                entityType: "User",
                entityId: resetToken.UserId,
                details: new { timestamp = DateTime.UtcNow }
            );
        }

        private static string GenerateSixDigitCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }

        public static string PasswordResetTemplate(string code) => $"""
            <!DOCTYPE html>
            <html>
            <body style="margin:0;padding:0;background:#f4f4f4;font-family:Arial,sans-serif;">
              <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                  <td align="center" style="padding:40px 0;">
                    <table width="480" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:12px;overflow:hidden;">
          
                      <!-- Header -->
                      <tr>
                        <td style="background:#1DB954;padding:32px;text-align:center;">
                          <h1 style="color:#ffffff;margin:0;font-size:24px;">SoloCoach</h1>
                        </td>
                      </tr>

                      <!-- Body -->
                      <tr>
                        <td style="padding:32px;">
                          <h2 style="color:#222;margin:0 0 12px;">Восстановление пароля</h2>
                          <p style="color:#555;font-size:15px;line-height:1.6;">
                            Вы запросили сброс пароля. Введите код ниже в приложении:
                          </p>
              
                          <!-- Code -->
                          <div style="text-align:center;margin:32px 0;">
                            <span style="font-size:40px;font-weight:bold;letter-spacing:12px;color:#1DB954;">
                              {code}
                            </span>
                          </div>
              
                          <p style="color:#888;font-size:13px;text-align:center;">
                            Код действителен 15 минут.<br/>
                            Если вы не запрашивали сброс пароля — проигнорируйте это письмо.
                          </p>
                        </td>
                      </tr>

                      <!-- Footer -->
                      <tr>
                        <td style="background:#f4f4f4;padding:16px;text-align:center;">
                          <p style="color:#aaa;font-size:12px;margin:0;">
                            © 2025 SoloCoach. Все права защищены.
                          </p>
                        </td>
                      </tr>

                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """;
    }
}
