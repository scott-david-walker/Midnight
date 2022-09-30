#region

using System;
using System.Threading.Tasks;

#endregion

namespace Midnight.Storage.Blobs.Internal;

public interface IMidnightBlobRetriever
{
    Task<BinaryData> Get(string blobLocation, string containerName);
}