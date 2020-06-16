using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InReachProject.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace InReachProject.Pages
{

    public class IndexModel : PageModel
    {
        //File URL on S3
        public string URL = "";

        //Message and class strings for error and success
        public string Message;
        public string msgClass = "";

        //Service and environment variables
        private readonly Services.IS3Service _is3Serv;
        private readonly Services.MailService _mailServ;
        private readonly ILogger<IndexModel> _logger;
        private IWebHostEnvironment _env;

        //S3 Bucket where files will be uploaded
        private string bucketName = "inreachprojectbucket";

        public IndexModel(ILogger<IndexModel> logger, Services.IS3Service is3Serv,
            Services.MailService mailServ, IWebHostEnvironment env)
            
        {
            _logger = logger;
            _is3Serv = is3Serv;
            _mailServ = mailServ;
            _env = env;
        }

        public void OnGet()
        {
            Message = "";
        }

        public void OnPost(string email, IFormFile file)
        {
            //Verify that email is valid and that file is not null
            bool isValid = false;

            if (email != null)
            {
                isValid = new EmailAddressAttribute().IsValid(email);
            }
            if (isValid && file != null)
            {
                //Upload file and set success message

                UploadForm(file, email);
                Message = "File has been uploaded and a confirmation has " +
                    "been sent to " + email + ". \n Your file can be accessed here:";
                msgClass = "text-success";

            }
            else
            {
                //Set error message and class
                Message = "Please choose a file and enter a valid email.";
                msgClass = "text-danger";
            }
            
        }

        public void UploadForm(IFormFile file, string email)
        {
            //Create unique filename for server upload
            string webRootPath = _env.WebRootPath;
            string fileName = Path.GetFileName(file.FileName);
            string uploadFolder = Path.Combine(webRootPath, "files");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            //Copy file to server
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            //Add file to S3
            AddFileToS3(bucketName, filePath);

            //Create presigned URL for S3 file
            string url = _is3Serv.GeneratePreSignedURL(bucketName, uniqueFileName);


            //Email the url to user
            _mailServ.SendMail(email, url);

            URL = url;
        }

        //Call service for uploading to S3
        public async void AddFileToS3(string bucketName, string fileName)
{
            await _is3Serv.UploadFileAsync(bucketName, fileName);

        }
    }
}
