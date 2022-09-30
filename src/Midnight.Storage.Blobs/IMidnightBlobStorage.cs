using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Midnight.Storage.Blobs;

public interface IMidnightBlobStorage
{
    BlobServiceClient GetServiceClient();
    IMidnightBlobStorage UseContainer(string containerName);
    Task<BinaryData> Get(string blobLocation);
    Task<BinaryData> Get(string blobLocation, string containerName);
    Task Save(BinaryData content, string blobLocation);
    Task Save(BinaryData content, string blobLocation, string containerName);
}