using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FileController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
                _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ??
                throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }

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

            if(!_fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(bytes, contentType, Path.GetFileName(filePath));
        }
    }
}
