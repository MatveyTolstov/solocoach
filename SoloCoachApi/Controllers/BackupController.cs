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

        private string FindPsql()
        {
            var commonPaths = new[]
            {
                "psql",
                @"E:\Postgre\bin\psql.exe",
                @"E:\PostgreSql\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\18\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\17\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\16\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\15\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\14\bin\psql.exe",
                @"C:\Program Files\PostgreSQL\13\bin\psql.exe",
            };

            foreach (var path in commonPaths)
            {
                try { if (path == "psql" || System.IO.File.Exists(path)) return path; }
                catch { }
            }

            throw new FileNotFoundException("psql не найдена. Убедитесь, что PostgreSQL установлен и добавлен в PATH.");
        }

        private static string FindPgDump()
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

        [HttpPost("restore")]
        public async Task<IActionResult> RestoreBackup(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Файл не передан." });

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                return BadRequest(new { message = "Строка подключения не настроена" });

            var connBuilder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
            var tempPath = Path.Combine(Path.GetTempPath(), $"restore_{Guid.NewGuid()}.sql");

            try
            {
                await using (var stream = System.IO.File.Create(tempPath))
                    await file.CopyToAsync(stream);

                var psqlPath = FindPsql();

                var psi = new ProcessStartInfo
                {
                    FileName = psqlPath,
                    Arguments = $"-h {connBuilder.Host} -p {connBuilder.Port} -U {connBuilder.Username} -d {connBuilder.Database} -f \"{tempPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                if (!string.IsNullOrEmpty(connBuilder.Password))
                    psi.Environment["PGPASSWORD"] = connBuilder.Password;

                using var process = Process.Start(psi);
                if (process == null)
                    return BadRequest(new { message = "Не удалось запустить psql" });

                if (!process.WaitForExit(300000))
                {
                    process.Kill();
                    return BadRequest(new { message = "Истекло время ожидания восстановления" });
                }

                if (process.ExitCode != 0)
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    _logger.LogError("Restore failed: {Error}", error);
                    return BadRequest(new { message = "Восстановление не удалось", error });
                }

                return Ok(new { message = "База данных успешно восстановлена" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring backup");
                return BadRequest(new { message = ex.Message });
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                    try { System.IO.File.Delete(tempPath); } catch { }
            }
        }
    }
}
