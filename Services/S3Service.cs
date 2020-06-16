using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using InReachProject.Models;

namespace InReachProject.Services
{
    public class S3Service : IS3Service
    {
        private static IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }

        /*Creates S3 bucket. Not used for this project.
         * */
        public async Task<S3Response> CreateBucketAsync(string bucketName)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    var response = await _client.PutBucketAsync(putBucketRequest);

                    return new S3Response
                    {
                        Message = response.ResponseMetadata.RequestId,
                        Status = response.HttpStatusCode
                    };
                }
            }
            catch (AmazonS3Exception e)
            {
                return new S3Response
                {
                    Status = e.StatusCode,
                    Message = e.Message
                };
            }
            catch (Exception e)
            {
                return new S3Response
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = e.Message
                };
            }

            return new S3Response
            {
                Status = HttpStatusCode.InternalServerError,
                Message = "Something went wrong"
            };
        }

        //Uploads file to S3
         
        public async Task UploadFileAsync(string bucketName, string filePath)
        {
            try 
            {
                var fileTransferUtility = new TransferUtility(_client);

                await fileTransferUtility.UploadAsync(filePath, bucketName);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        //Creates Presigned URL for file in S3 bucket
        public string GeneratePreSignedURL(string bucketName, string objectKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = DateTime.Now.AddMinutes(60)
            };

            var url = _client.GetPreSignedURL(request);
            return url;
        }

    }
}
