#region

using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

#endregion

namespace Midnight.Storage.Blobs.Internal;

internal class MidnightBlobRetriever : IMidnightBlobRetriever
{
    private readonly IMidnightContainerRetriever _midnightContainerRetriever;

    public MidnightBlobRetriever(IMidnightContainerRetriever midnightContainerService)
    {
        _midnightContainerRetriever = midnightContainerService;
    }

    public async Task<BinaryData> Get(string blobLocation, string containerName)
    {
        var container = await _midnightContainerRetriever.GetContainer(containerName);
        return await Get(blobLocation, container);
    }

    private static async Task<BinaryData> Get(string blobLocation, BlobContainerClient client)
    {
        var blob = client.GetBlobClient(blobLocation);
        var download = await blob.DownloadContentAsync();
        var content = download.Value.Content;
        return content;
    }
}