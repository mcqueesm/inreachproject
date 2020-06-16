using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InReachProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

/* Initially when creating this project I posted to this controller. The project
 * as it stands uses the Razor Pages PageModel to call these services, but I have
 * left this controller just to demonstrate another way of organizing this project.
 * 
 * I also experimented with adding buckets programatically through the CreateBucket
 * function.
 * */
namespace InReachProject.Controllers
{
    [Route("[controller]")]
    public class S3BucketController : Controller
    {
        private readonly Services.IS3Service _service;
        private IWebHostEnvironment _env;

        public S3BucketController(Services.IS3Service service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        

        [HttpPost("{bucketName}")]

        public async Task<IActionResult> CreateBucket([FromRoute] string bucketName)
        {
            var response = await _service.CreateBucketAsync(bucketName);

            return Ok(response);
        }

        [HttpPost("UploadForm")]
       public IActionResult UploadForm(IFormFile file)
        {
            string webRootPath = _env.WebRootPath;
            string fileName = Path.GetFileName(file.FileName);
            string uploadFolder = Path.Combine(webRootPath, "files");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
            string filePath = Path.Combine(uploadFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }
            return Redirect("/");
        }
    }
}
