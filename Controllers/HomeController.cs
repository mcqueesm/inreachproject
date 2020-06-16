using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



/* Initially when creating this project I posted to this controller. The project
 * as it stands uses the Razor Pages PageModel to call these services, but I have
 * left this controller just to demonstrate another way of organizing this project.
 * */
namespace InReachProject.Controllers
{
    //[Route("[controller]")]
    public class HomeController : Controller
    {
        private IWebHostEnvironment _env;
        private readonly Services.IS3Service _service;
        private Services.MailService _mailService;

        private string bucketName = "inreachprojectbucket";

        public HomeController(IWebHostEnvironment env, Services.IS3Service service,
            Services.MailService mailService)
        {
            _service = service;
            _mailService = mailService;
            _env = env;
        }

        public IActionResult Index() => View();

        [HttpPost("UploadForm")]
        public async Task<string> UploadFormAsync(IFormFile file, string email)
        {
            //Upload file to server before uploading to S3
            string webRootPath = _env.WebRootPath;
            string fileName = Path.GetFileName(file.FileName);
            string uploadFolder = Path.Combine(webRootPath, "files");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }
            
            await AddFileToS3(bucketName, filePath);

            string url = _service.GeneratePreSignedURL(bucketName, uniqueFileName);

            //_service.UploadObject(url, filePath);

            _mailService.SendMail(email, url);

            return url;
        }

        

        public async Task<IActionResult> AddFileToS3(string bucketName, string fileName)
         {
             await _service.UploadFileAsync(bucketName, fileName);

             return Ok();
         }
    }
}
