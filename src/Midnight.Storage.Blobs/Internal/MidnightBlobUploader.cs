#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Midnight.Storage.Blobs.Extensions;

#endregion

namespace Midnight.Storage.Blobs.Internal;

internal class MidnightBlobUploader : IMidnightBlobUploader
{
    private readonly IMidnightContainerRetriever _containerRetriever;
    private readonly MidnightBlobsBuilder _options;

    public MidnightBlobUploader(IMidnightContainerRetriever containerRetriever,
        IOptions<MidnightBlobsBuilder> settings)
    {
        _containerRetriever = containerRetriever;
        _options = settings.Value;
    }

    public async Task Save(BinaryData content, string blobLocation, string containerName)
    {
        var container = await _containerRetriever.GetContainer(containerName);
        var blob = container.GetBlobClient(blobLocation);
        await blob.UploadAsync(content, _options.ShouldOverwriteExistingBlob);
    }
}