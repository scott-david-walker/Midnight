#region

using System;
using System.Threading.Tasks;

#endregion

namespace Midnight.Storage.Blobs.Internal;

internal interface IMidnightBlobRetriever
{
    Task<BinaryData> Get(string blobLocation, string containerName);
}