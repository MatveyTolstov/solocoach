namespace SoloCoachApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.IO;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackupController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IConfiguration configuration, ILogger<BackupController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private string FindPgDump()
        {
            // Проверяем стандартные пути установки PostgreSQL на Windows
            var commonPaths = new[]
            {
                "pg_dump",
                @"E:\Postgre\bin\pg_dump.exe", // Локальная установка
                @"E:\PostgreSql\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\18\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\17\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\16\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\15\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\14\bin\pg_dump.exe",
                @"C:\Program Files\PostgreSQL\13\bin\pg_dump.exe",
                @"C:\Program Files (x86)\PostgreSQL\18\bin\pg_dump.exe",
                @"C:\Program Files (x86)\PostgreSQL\17\bin\pg_dump.exe",
                @"C:\Program Files (x86)\PostgreSQL\16\bin\pg_dump.exe",
                @"C:\Program Files (x86)\PostgreSQL\15\bin\pg_dump.exe",
                @"C:\Program Files (x86)\PostgreSQL\14\bin\pg_dump.exe",
            };

            //foreach (var path in commonPaths)
            //{
            //    try
            //    {
            //        if (System.IO.File.Exists(path))
            //        {
            //            _logger.LogInformation($"Found pg_dump at: {path}");
            //            return path;
            //        }
            //    }
            //    catch
            //    {
            //        // ignore
            //    }
            //}

            throw new FileNotFoundException("pg_dump не найдена. Убедитесь, что PostgreSQL установлен и добавлен в переменную PATH.");
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                // Получаем строку подключения из конфига
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    return BadRequest(new { message = "Строка подключения не настроена" });
                }

                // Парсим параметры подключения PostgreSQL
                var connBuilder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
                var host = connBuilder.Host;
                var port = connBuilder.Port;
                var database = connBuilder.Database;
                var username = connBuilder.Username;
                var password = connBuilder.Password;


                var pgDumpPath = FindPgDump();

                // Создаем имя файла бекапа с текущей датой и временем
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                var backupFileName = $"backup_{database}_{timestamp}.sql";
                var backupPath = Path.Combine(Path.GetTempPath(), backupFileName);

                var psi = new ProcessStartInfo
                {
                    FileName = pgDumpPath,
                    Arguments = $"-h {host} -p {port} -U {username} -d {database} -F p -f \"{backupPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                if (!string.IsNullOrEmpty(password))
                {
                    psi.Environment["PGPASSWORD"] = password;
                }

                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        return BadRequest(new { message = "Не удалось запустить процесс резервного копирования" });
                    }

                    // Ждем завершения процесса с timeout в 5 минут
                    if (!process.WaitForExit(300000))
                    {
                        process.Kill();
                        return BadRequest(new { message = "Истекло время ожидания процесса резервного копирования" });
                    }

                    if (process.ExitCode != 0)
                    {
                        var error = process.StandardError.ReadToEnd();
                        _logger.LogError($"Backup failed: {error}");
                        return BadRequest(new { message = "Резервное копирование не удалось", error });
                    }
                }

                if (!System.IO.File.Exists(backupPath))
                {
                    return BadRequest(new { message = "Файл резервной копии не был создан" });
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(backupPath);

                // Удаляем временный файл
                try
                {
                    System.IO.File.Delete(backupPath);
                }
                catch
                {
                    // игнорируем ошибки при удалении временного файла
                }

                return new FileContentResult(fileBytes, "application/octet-stream")
                {
                    FileDownloadName = backupFileName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating backup");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
