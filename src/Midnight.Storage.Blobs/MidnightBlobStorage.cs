using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Midnight.Storage.Blobs.Extensions;

namespace Midnight.Storage.Blobs;

internal class MidnightBlobStorage : IMidnightBlobStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly MidnightBlobsBuilder _options;
    private string _containerName;
    public MidnightBlobStorage(IAzureClientFactory<BlobServiceClient> factory,
        IOptions<MidnightBlobsBuilder> options)
    {
        _options = options.Value;
        _blobServiceClient = factory.CreateClient(AzureClientNaming.MidnightBlobName);
    }

    public BlobServiceClient GetServiceClient()
    {
        return _blobServiceClient;
    }

    public IMidnightBlobStorage UseContainer(string containerName)
    {
        _containerName = containerName;
        return this;
    }

    public async Task<BinaryData> Get(string blobLocation)
    {
        return await Get(blobLocation, _containerName);
    }
    
    public async Task<BinaryData> Get(string blobLocation, string containerName)
    {
        var container = await GetContainer(containerName);
        return await Get(blobLocation, container);
    }
    
    public async Task Save(BinaryData content, string blobLocation)
    {
        await Save(content, blobLocation, _containerName);
    }
    
    public async Task Save(BinaryData content, string blobLocation, string containerName)
    {
        var container = await GetContainer(containerName);
        var blob = container.GetBlobClient(blobLocation);
        await blob.UploadAsync(content, _options.ShouldOverwriteExistingBlob);
    }

    private static async Task<BinaryData> Get(string blobLocation, BlobContainerClient client)
    {
        var blob = client.GetBlobClient(blobLocation);
        var download = await blob.DownloadContentAsync();
        var content = download.Value.Content;
        return content;
    }

    private async Task<BlobContainerClient> GetContainer(string containerName)
    {
        await CreateContainerIfNecessary(containerName);
        return _blobServiceClient.GetBlobContainerClient(containerName);
    }
    
    private async Task CreateContainerIfNecessary(string containerName)
    {
        ThrowIfContainerNameIsNull(containerName);
        if (_options.ShouldCreateContainerIfNotExisting)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
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
