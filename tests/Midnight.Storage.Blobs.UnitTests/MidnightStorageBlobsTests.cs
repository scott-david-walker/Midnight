using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Midnight.Storage.Blobs.Extensions;
using Moq;

namespace Midnight.Storage.Blobs.UnitTests;

public class MidnightStorageBlobsTests
{
    private readonly Mock<IAzureClientFactory<BlobServiceClient>> _clientFactoryMock = new();
    private readonly Mock<BlobServiceClient> _blobServiceClientMock = new();
    private readonly Mock<IOptions<MidnightBlobsBuilder>> _optionsMock = new();
    private readonly Fixture _fixture = new();
    public MidnightStorageBlobsTests()
    {
        _clientFactoryMock.Setup(t => t.CreateClient("MidnightBlobs"))
            .Returns(_blobServiceClientMock.Object);
                
        _optionsMock.Setup(m => m.Value).Returns(new MidnightBlobsBuilder());
    }

    [Fact]
    public void GivenGettingBlobServiceMock_ShouldReturnObjectCreatedInConstructor()
    {
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        var client = sut.GetServiceClient();
        client.Should().Be(_blobServiceClientMock.Object);
    }

    [Fact]
    public async Task GivenGet_WhenUseContainerIsSpecified_ShouldReturnBlob()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var azureResponseMock = new Mock<Response<BlobDownloadResult>>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();

        _blobServiceClientMock.Setup(t => t.GetBlobContainerClient(container))
            .Returns(containerMock.Object);;

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);

        var data =  BlobsModelFactory.BlobDownloadResult(new BinaryData("test"));
        azureResponseMock.Setup(t => t.Value).Returns(data);
        blobMock.Setup(t => t.DownloadContentAsync()).ReturnsAsync(azureResponseMock.Object);
        
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        sut.UseContainer(container);
        var result = await sut.Get(blobLocation);
        result.Should().Be(data.Content);
    }

    [Fact]
    public async Task GivenGet_WhereUseContainerIsNotUsed_ShouldThrowNullReferenceException()
    { 
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        await sut.Invoking(t => t.Get(""))
            .Should()
            .ThrowAsync<NullReferenceException>();
    }
    
    [Fact]
    public async Task GivenGet_WhenContainerNameIsProvidedAsArgument_ShouldReturnBlob()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var azureResponseMock = new Mock<Response<BlobDownloadResult>>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();

        _blobServiceClientMock.Setup(t => t.GetBlobContainerClient(container))
            .Returns(containerMock.Object);;

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);

        var data =  BlobsModelFactory.BlobDownloadResult(new BinaryData("test"));
        azureResponseMock.Setup(t => t.Value).Returns(data);
        blobMock.Setup(t => t.DownloadContentAsync()).ReturnsAsync(azureResponseMock.Object);
        
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        var result = await sut.Get(blobLocation, container);
        result.Should().Be(data.Content);
    }
    
    [Fact]
    public async Task GivenGet_WhenOptionSpecifiesShouldEnsureCreated_ShouldCallCreateIfNotExists()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var azureResponseMock = new Mock<Response<BlobDownloadResult>>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();

        _blobServiceClientMock.Setup(t => t.GetBlobContainerClient(container))
            .Returns(containerMock.Object);;

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);

        var data =  BlobsModelFactory.BlobDownloadResult(new BinaryData("test"));
        azureResponseMock.Setup(t => t.Value).Returns(data);
        blobMock.Setup(t => t.DownloadContentAsync()).ReturnsAsync(azureResponseMock.Object);
        _optionsMock.Setup(m => m.Value).Returns(new MidnightBlobsBuilder
        {
            ShouldCreateContainerIfNotExisting = true
        });
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        await sut.Get(blobLocation, container);
        containerMock.Verify(t=>t.CreateIfNotExistsAsync(default, null, null, default), Times.Once);
    }

    [Fact]
    public async Task GivenSave_IfUseContainerIsNotUsed_ShouldThrowNullReferenceException()
    {
        var blobLocation = _fixture.Create<string>();
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        await sut.Invoking(t => t.Save(new BinaryData(""), blobLocation))
            .Should()
            .ThrowAsync<NullReferenceException>();
    }
    
    [Fact]
    public async Task GivenSave_IfUseContainerUsed_ShouldSaveBlobToContainer()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();
        var content = _fixture.Create<string>();

        _blobServiceClientMock.Setup(t => t.GetBlobContainerClient(container))
            .Returns(containerMock.Object);;

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);
        
        var data = new BinaryData(content);
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        sut.UseContainer(container);
        await sut.Save(data, blobLocation);
        blobMock.Verify(t=>t.UploadAsync(data, false, default), Times.Once);
    }
    
    [Fact]
    public async Task GivenSave_WhenOverwriteIsSetInSettings_ShouldSaveBlobToContainerWithOverwrite()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();
        var content = _fixture.Create<string>();

        _blobServiceClientMock.Setup(t => t.GetBlobContainerClient(container))
            .Returns(containerMock.Object);;

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);
        
        var data = new BinaryData(content);
        
        _optionsMock.Setup(m => m.Value).Returns(new MidnightBlobsBuilder
        {
            ShouldOverwriteExistingBlob = true
        });
        
        var sut = new MidnightBlobStorage(_clientFactoryMock.Object, _optionsMock.Object);
        sut.UseContainer(container);
        await sut.Save(data, blobLocation);
        blobMock.Verify(t=>t.UploadAsync(data, true, default), Times.Once);
    }
}