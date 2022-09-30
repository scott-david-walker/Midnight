#region

using System;
using System.Threading.Tasks;

#endregion

namespace Midnight.Storage.Blobs.Internal;

internal interface IMidnightBlobUploader
{
    Task Save(BinaryData content, string blobLocation, string containerName);
}