using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Mock.Json.Controllers
{
    public class JsonHandlerController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public JsonHandlerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> GetJsonAsync(string filename)
        {
            var clientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string filePath = Path.Combine(_configuration["JsonHandler:FolderPath"], $"{filename}.json");

            if (!System.IO.File.Exists(filePath))
            {
                var errorMessage = new { message = $"File with name '{filename}' not found" };
                return NotFound(errorMessage);
            }

            string jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
            return Content(jsonContent, "application/json");
        }

        [HttpGet("with-folder")]
        public async Task<IActionResult> GetJsonWithFolderPathAsync(string folder, string filename)
        {
            var clientIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var folderPath = Path.Combine(_configuration["JsonHandler:FolderPath"], folder);
            string filePath = Path.Combine(folderPath, $"{filename}.json");

            if (!Directory.Exists(folderPath) || !System.IO.File.Exists(filePath))
            {
                var errorMessage = new { message = $"File with folder '{folder}' and name '{filename}' not found" };
                return NotFound(errorMessage);
            }

            string jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
            return Content(jsonContent, "application/json");
        }

        [HttpGet("random/{folder}")]
        public async Task<IActionResult> GetRandomJsonFromFolder(string folder)
        {
            try
            {
                var folderPath = Path.Combine(_configuration["JsonHandler:FolderPath"], folder);

                if (!Directory.Exists(folderPath))
                {
                    var errorMessage = new { message = $"Folder '{folder}' not found" };
                    return NotFound(errorMessage);
                }

                var jsonFiles = Directory.GetFiles(folderPath, "*.json");

                if (jsonFiles.Length == 0)
                {
                    var errorMessage = new { message = $"No files found in folder '{folder}'" };
                    return NotFound(errorMessage);
                }

                var random = new Random();
                var randomFilePath = jsonFiles[random.Next(jsonFiles.Length)];

                string jsonContent = await System.IO.File.ReadAllTextAsync(randomFilePath);
                return Content(jsonContent, "application/json");
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred while processing the request" });
            }
        }
    }
}
