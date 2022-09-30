using System;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Midnight.Storage.Blobs.Extensions;

public static class BuilderExtensions
{
    public static IServiceCollection AddMidnightBlobs(this IServiceCollection services, string storageConnectionString, Action<MidnightBlobsBuilder> options = null)
    {
        services.Configure(options);
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(storageConnectionString)
                .WithName(AzureClientNaming.MidnightBlobName);
        });

        services.AddScoped<IMidnightBlobStorage, MidnightBlobStorage>();
        return services;
    }
}

public class MidnightBlobsBuilder
{
    public bool ShouldCreateContainerIfNotExisting { get; set; }
    public bool ShouldOverwriteExistingBlob { get; set; }
}