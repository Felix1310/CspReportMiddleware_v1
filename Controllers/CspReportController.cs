using CspReportMiddleware.Config;
using CspReportMiddleware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CspReportMiddleware.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CspReportController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly CspSettings _settings;

        public CspReportController(IWebHostEnvironment env, IOptions<CspSettings> settings)
        {
            _env = env;
            _settings = settings.Value;
        }

        [HttpPost]
        [Route("/cspreport")]
        public async Task<IActionResult> ReceiveReport()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var report = JsonSerializer.Deserialize<CspReportWrapper>(body);

            if (report?.CspReport == null)
            {
                Console.WriteLine("⚠️ Invalid CSP report received.");
                return BadRequest("Invalid report.");
            }

            var basePath = Path.IsPathRooted(_settings.ReportSavePath)
                ? _settings.ReportSavePath
                : Path.Combine(_env.ContentRootPath, _settings.ReportSavePath);

            var dateFolder = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var dailyPath = Path.Combine(basePath, dateFolder);

            Directory.CreateDirectory(dailyPath);

            var fileName = $"csp-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.json";
            var filePath = Path.Combine(dailyPath, fileName);

            var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await System.IO.File.WriteAllTextAsync(filePath, json);

            Console.WriteLine($"✅ CSP Report saved to: {filePath}");

            return Ok();
        }
    }
}
