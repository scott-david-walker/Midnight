# Midnight.Storage.BLobs

A simple wrapper around the Azure.Storage.Blobs library that exposes basic functionality such as get and save

## Configuration Options

When configuring Midnight as a service, you can add a builder action:

```
builder.Services.AddMidnightBlobs("Blob Storage Connection String", t =>
{
    t.ShouldCreateContainerIfNotExisting = true;
    t.ShouldOverwriteExistingBlob = true;
});
```



```
ShouldCreateContainerIfNotExisting
```

When getting or saving to a container, setting this value to true will ensure that the container always exists
and will create it if it doesn't exist. If set to false, an exception will be thrown.

```
ShouldOverwriteExistingBlob
```

When saving a blob, will overwrite the blob if set to true. Otherwise an exception will be thrown

## Usage

Inject IMidnight blob storage to use the available functions.

Each function has an overload for a container name. 
If you're using the same container for multiple calls, you can call "UseContainer" to set the container for all following calls as long as you don't specify the container.

If you want access to built in Azure functionalities that aren't wrapped in this library, 
you can call "GetServiceClient" to access the existing Azure API's 