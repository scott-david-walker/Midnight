#region

using System;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Midnight.Storage.Blobs.Internal;

#endregion

namespace Midnight.Storage.Blobs.Extensions;

public static class BuilderExtensions
{
    public static IServiceCollection AddMidnightBlobs(this IServiceCollection services, string storageConnectionString, Action<MidnightBlobsBuilder> options = null)
    {
        if (options != null)
        {
            services.Configure(options);    
        }
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(storageConnectionString)
                .WithName(AzureClientNaming.MidnightBlobName);
        });

        services.AddScoped<IMidnightBlobStorage, MidnightBlobStorage>();
        services.AddScoped<IMidnightContainerService, MidnightContainerService>();
        services.AddScoped<IMidnightContainerRetriever, MidnightContainerRetriever>();
        services.AddScoped<IMidnightBlobUploader, MidnightBlobUploader>();
        services.AddScoped<IMidnightBlobRetriever, MidnightBlobRetriever>();


        return services;
    }
}

public class MidnightBlobsBuilder
{
    public bool ShouldCreateContainerIfNotExisting { get; set; }
    public bool ShouldOverwriteExistingBlob { get; set; }
}