#region

using AutoFixture;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Midnight.Storage.Blobs.Internal;
using Moq;

#endregion

namespace Midnight.Storage.Blobs.UnitTests;

public class GetBlobsTests
{
    private readonly Mock<IMidnightContainerRetriever> _containerRetrieverMock = new();
    private readonly MidnightBlobRetriever _sut;
    private readonly Fixture _fixture = new();

    public GetBlobsTests()
    {
        _sut = new MidnightBlobRetriever(_containerRetrieverMock.Object);
    }

    [Fact]
    public async Task GivenGet_ShouldReturnBlob()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var azureResponseMock = new Mock<Response<BlobDownloadResult>>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();

        _containerRetrieverMock.Setup(t => t.GetContainer(container))
            .ReturnsAsync(containerMock.Object);
        ;

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);

        var data = BlobsModelFactory.BlobDownloadResult(new BinaryData("test"));
        azureResponseMock.Setup(t => t.Value).Returns(data);
        blobMock.Setup(t => t.DownloadContentAsync()).ReturnsAsync(azureResponseMock.Object);

        var result = await _sut.Get(blobLocation, container);
        result.Should().Be(data.Content);
    }
}