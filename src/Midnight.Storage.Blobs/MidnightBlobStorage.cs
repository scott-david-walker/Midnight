#region

using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Midnight.Storage.Blobs.Internal;

#endregion

namespace Midnight.Storage.Blobs;

internal class MidnightBlobStorage : IMidnightBlobStorage, IMidnightContainerService, IMidnightBlobRetriever,
    IMidnightBlobUploader
{
    private readonly IMidnightContainerService _containerService;
    private readonly IMidnightBlobRetriever _midnightBlobRetriever;
    private readonly IMidnightBlobUploader _midnightBlobUploader;
    private string _container;

    public MidnightBlobStorage(IMidnightContainerService containerService,
        IMidnightBlobRetriever midnightBlobRetriever,
        IMidnightBlobUploader midnightBlobUploader)
    {
        _containerService = containerService;
        _midnightBlobRetriever = midnightBlobRetriever;
        _midnightBlobUploader = midnightBlobUploader;
    }

    /// <summary>
    ///     A method that will return a BlobServiceClient giving direct access to the Azure Api
    /// </summary>
    /// <returns>Blob Service Client</returns>
    public BlobServiceClient GetServiceClient()
    {
        return _containerService.GetServiceClient();
    }

    /// <summary>
    ///     Sets the container for any subsequent calls
    /// </summary>
    /// <param name="containerName"></param>
    public void UseContainer(string containerName)
    {
        _container = containerName;
    }
    
    public async Task<BinaryData> Get(string blobLocation)
    {
        return await _midnightBlobRetriever.Get(blobLocation, _container);
    }
    
    public async Task<BinaryData> Get(string blobLocation, string containerName)
    {
        return await _midnightBlobRetriever.Get(blobLocation, containerName);
    }
    
    public async Task Save(BinaryData content, string blobLocation)
    {
        await _midnightBlobUploader.Save(content, blobLocation, _container);
    }
    
    public async Task Save(BinaryData content, string blobLocation, string containerName)
    {
        await _midnightBlobUploader.Save(content, blobLocation, containerName);
    }
}
