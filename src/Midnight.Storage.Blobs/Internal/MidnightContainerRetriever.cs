#region

using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Midnight.Storage.Blobs.Extensions;

#endregion

namespace Midnight.Storage.Blobs.Internal;

internal class MidnightContainerRetriever : IMidnightContainerRetriever
{
    private readonly IMidnightContainerService _midnightContainerService;
    private readonly MidnightBlobsBuilder _options;

    public MidnightContainerRetriever(IMidnightContainerService midnightContainerService,
        IOptions<MidnightBlobsBuilder> settings)
    {
        _midnightContainerService = midnightContainerService;
        _options = settings.Value;
    }

    public async Task<BlobContainerClient> GetContainer(string containerName)
    {
        await CreateContainerIfNecessary(containerName);
        return _midnightContainerService.GetServiceClient().GetBlobContainerClient(containerName);
    }

    private async Task CreateContainerIfNecessary(string containerName)
    {
        ThrowIfContainerNameIsNull(containerName);
        if (_options.ShouldCreateContainerIfNotExisting)
        {
            var containerClient = _midnightContainerService.GetServiceClient().GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
        }
    }

    private static void ThrowIfContainerNameIsNull(string containerName)
    {
        if (containerName == null)
        {
            throw new NullReferenceException("No container name specified");
        }
    }
}