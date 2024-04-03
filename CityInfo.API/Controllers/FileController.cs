using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileController : Controller
    {
        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId) 
        {
            //Get File based on fileId - I did some sample 
            var filePath = "";

            if (fileId == "1")
            {
                filePath = "Files/creating-the-api-and-returning-resources-slides.pdf";

            }

            else
            {
                filePath = "Files/getting-acquainted-with-aspnet-core-slides.pdf";
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "text/plain", Path.GetFileName(filePath));
        }
    }
}
