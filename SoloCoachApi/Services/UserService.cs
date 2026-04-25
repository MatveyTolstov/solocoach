using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;
using SoloCoachApi.Repositories;
using System.CodeDom.Compiler;

namespace SoloCoachApi.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ApplicationContext _context;

        public UserService(IUserRepository userRepository, IEmailService emailService, ApplicationContext context)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _context = context;
        }

        public Task<UserDto> GetByIdAsync(int id) => _userRepository.GetUserByIdAsync(id);

        public Task<List<UserDto>> GetAllAsync() => _userRepository.GetAllUsersAsync();

        public Task<UserDto> CreateAsync(UserDto dto) => _userRepository.CreateUserAsync(dto);

        public Task<UserDto> UpdateAsync(UserDto dto) => _userRepository.UpdateUserAsync(dto);

        public Task DeleteAsync(int id) => _userRepository.DeleteUserAsync(id);

        public async Task<bool> CheckEmailTaken(int id, string email)
        {
            return await _userRepository.IsEmailTakenAsync(id, email);
        }


        public string GeneratedCode()
        {
            var code = new Random().Next(100000, 999999).ToString();
            return code;
        }

        public async Task SendEmailCode(int userId,RequestEmailChangeDto request)
        {
            if (await CheckEmailTaken(userId, request.NewEmail))
            {
                throw new ArgumentException("Этот email уже занят");
            }

            var userEmail = await _context.Users
                .Where(u => u.IdUser == userId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (userEmail == request.NewEmail)
            {
                throw new ArgumentException("Новый email совпадает с текущим");
            }

            var oldCodes = _context.EmailChangeCodes
                .Where(c => c.UserId == userId && !c.IsUsed);
            _context.EmailChangeCodes.RemoveRange(oldCodes);

            var code = GeneratedCode();

            _context.EmailChangeCodes.Add(new EmailChangeCode
            {
                UserId = userId,
                NewEmail = request.NewEmail,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            });

            await _context.SaveChangesAsync();


            await _emailService.SendEmailAsync(
                request.NewEmail,
                "Подтверждение изменения email — SoloCoach",
                EmailChangeTemplate(code)
            );
        }

        public async Task ConfirmEmailChange(int userId, ConfirmEmailChangeDto request)
        {
            var codeEntry = await _context.EmailChangeCodes
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Code == request.Code && !c.IsUsed);

            if (codeEntry == null)
                throw new ArgumentException("Неверный код");

            if (codeEntry.ExpiresAt < DateTime.UtcNow)
                throw new ArgumentException("Код просрочен");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");
            }
            user.Email = codeEntry.NewEmail;
            codeEntry.IsUsed = true;
            await _context.SaveChangesAsync();
        }

        public static string EmailChangeTemplate(string code) => $"""
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
                              <h2 style="color:#222;margin:0 0 12px;">Подтверждение email</h2>
                              <p style="color:#555;font-size:15px;line-height:1.6;">
                                Вы запросили изменение email адреса. Введите код ниже в приложении:
                              </p>
              
                              <!-- Code -->
                              <div style="text-align:center;margin:32px 0;">
                                <span style="font-size:40px;font-weight:bold;letter-spacing:12px;color:#1DB954;">
                                  {code}
                                </span>
                              </div>
              
                              <p style="color:#888;font-size:13px;text-align:center;">
                                Код действителен 15 минут.<br/>
                                Если вы не запрашивали смену email — проигнорируйте это письмо.
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

