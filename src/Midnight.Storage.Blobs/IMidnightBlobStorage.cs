#region

using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

#endregion

namespace Midnight.Storage.Blobs;

public interface IMidnightBlobStorage
{
    BlobServiceClient GetServiceClient();
    void UseContainer(string containerName);
    Task<BinaryData> Get(string blobLocation);
    Task<BinaryData> Get(string blobLocation, string containerName);
    Task Save(BinaryData content, string blobLocation);
    Task Save(BinaryData content, string blobLocation, string containerName);
}