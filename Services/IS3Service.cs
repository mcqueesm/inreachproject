using System.Threading.Tasks;
using InReachProject.Models;

namespace InReachProject.Services
{
    /*Interface for S3Service
     * */

    public interface IS3Service
    {
        Task<S3Response> CreateBucketAsync(string bucketName);

        Task UploadFileAsync(string bucketName, string fileName);

        string GeneratePreSignedURL(string bucketName, string objectKey);
    }
}
