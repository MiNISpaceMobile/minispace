using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.PictureStorages
{
    public class AzureBlobStorage : IStorage
    {
        public string RootUrl { get; }

        private BlobServiceClient client;

        public AzureBlobStorage(IConfiguration config)
        {
            string accountName = config["AZURE_BLOB_ACCOUNT_NAME"]!;

            RootUrl = $"https://{accountName}.blob.core.windows.net";

            StorageSharedKeyCredential credential =
                new StorageSharedKeyCredential(accountName, config["AZURE_BLOB_ACCOUNT_KEY"]);

            client = new BlobServiceClient(new Uri(RootUrl), credential);
        }

        public bool TryCreateDirectory(string directory)
        {
            try
            {
                client.CreateBlobContainer(directory, PublicAccessType.Blob);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool TryDeleteDirectory(string directory)
        {
            try
            {
                client.DeleteBlobContainer(directory);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string UploadFile(Stream file, string directory, string filename)
        {
            TryCreateDirectory(directory);
            try
            {
                var blob = client.GetBlobContainerClient(directory).GetBlobClient(filename);
                blob.Upload(file);
                return blob.Uri.AbsoluteUri;
            }
            catch
            {
                throw new Exception("Could not upload Blob");
            }
        }

        public string RenameFile(string directory, string oldFilename, string newFilename)
        {
            try
            {
                var source = client.GetBlobContainerClient(directory).GetBlobClient(oldFilename);
                var target = client.GetBlobContainerClient(directory).GetBlobClient(newFilename);
                target.SyncCopyFromUri(source.Uri);
                source.Delete();
                return target.Uri.AbsoluteUri;
            }
            catch
            {
                throw new Exception("Could not find/copy/delete Blob");
            }
        }

        public void DeleteFile(string directory, string filename)
        {
            try
            {
                client.GetBlobContainerClient(directory).GetBlobClient(filename).Delete();
            }
            catch
            {
                throw new Exception("Could not find/delete Blob");
            }
        }
    }
}
