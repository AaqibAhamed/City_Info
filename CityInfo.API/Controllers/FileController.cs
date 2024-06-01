using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/files")]
    [ApiController]
    [Authorize]
    public class FileController : Controller
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FileController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
                _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ??
                throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }

        [HttpGet("{fileId}")]
        [ApiVersion(0.1, Deprecated =true)]
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

        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            // Validate the input. Put a limit on filesize to avoid large uploads attacks. 
            // Only accept .pdf files (check content-type)

            if(file.Length == 0 || file.Length >20971520|| file.ContentType !="application/pdf")
            {
                return BadRequest("No file or an invalid one has been inputted.");
            }

            // Create the file path.  Avoid using file.FileName, as an attacker can provide a
            // malicious one, including full paths or relative paths.  

            var path = Path.Combine(Directory.GetCurrentDirectory(), $"upload/uploaded_file{Guid.NewGuid()}.pdf");

            using(var stream = new FileStream(path,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("Your file has been uploaded successfully."); 
        }
    }
}
