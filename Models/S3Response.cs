using System;
using System.Net;

namespace InReachProject.Models
{
    /*Class for response status and message after creating a new bucket with
     * CreateBucketAsync service (S3Service).
     * */
    public class S3Response
    {
        public HttpStatusCode Status { get; set; }

        public string Message { get; set; }
    }
}
