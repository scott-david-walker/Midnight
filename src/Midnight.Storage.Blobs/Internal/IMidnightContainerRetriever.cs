#region

using System.Threading.Tasks;
using Azure.Storage.Blobs;

#endregion

namespace Midnight.Storage.Blobs.Internal;

internal interface IMidnightContainerRetriever
{
    Task<BlobContainerClient> GetContainer(string containerName);
}