#region

using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Midnight.Storage.Blobs.Extensions;

#endregion

namespace Midnight.Storage.Blobs.Internal;

internal interface IMidnightContainerService
{
    BlobServiceClient GetServiceClient();
}

internal class MidnightContainerService : IMidnightContainerService
{
    private readonly BlobServiceClient _blobServiceClient;

    public MidnightContainerService(IAzureClientFactory<BlobServiceClient> factory
    )
    {
        _blobServiceClient = factory.CreateClient(AzureClientNaming.MidnightBlobName);
    }

    public BlobServiceClient GetServiceClient()
    {
        return _blobServiceClient;
    }
}