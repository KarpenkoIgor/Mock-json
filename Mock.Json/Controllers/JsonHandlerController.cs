using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    }
}
