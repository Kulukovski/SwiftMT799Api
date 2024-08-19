using Microsoft.AspNetCore.Mvc;
using SwiftMT799Api.Services;

namespace SwiftMT799Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MT799Controller : ControllerBase
    {
        private readonly MT799Parser _parser;
        private readonly SQLiteHelper _dbHelper;

        public MT799Controller(MT799Parser parser, SQLiteHelper dbHelper)
        {
            _parser = parser;
            _dbHelper = dbHelper;
        }

        [HttpPost]
        public IActionResult UploadMT799(IFormFile file = null, string messageString = null)
        {
            if (file != null && file.Length > 0)
            {
                // Handle file input
                using var reader = new StreamReader(file.OpenReadStream());
                var fileContent = reader.ReadToEnd();
                var message = _parser.Parse(fileContent);
                _dbHelper.InsertMessage(message);
            }
            else if (!string.IsNullOrEmpty(messageString))
            {
                // Handle direct string input
                var message = _parser.Parse(messageString);
                _dbHelper.InsertMessage(message);
            }
            else
            {
                return BadRequest("No file or message string provided.");
            }

            return Ok("MT799 message processed and saved.");
        }

        [HttpGet("Messages")]
        public IActionResult GetMessages()
        {
            return Ok(_dbHelper.GetAllMessages());
        }

        [HttpGet("HeaderMessages")]
        public IActionResult GetHeaderMessages()
        {
            return Ok(_dbHelper.GetAllGeneralHeaderData());
        }

        [HttpGet("InputHeaderMessages")]
        public IActionResult GetInputHeaderMessages()
        {
            return Ok(_dbHelper.GetAllInputHeaderData());
        }

        [HttpGet("OutputHeaderMessages")]
        public IActionResult GetOutputHeaderMessages()
        {
            return Ok(_dbHelper.GetAllOutputHeaderData());
        }
    }

}
