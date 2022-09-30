#region

using AutoFixture;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Midnight.Storage.Blobs.Extensions;
using Midnight.Storage.Blobs.Internal;
using Moq;

#endregion

namespace Midnight.Storage.Blobs.UnitTests;

public class SaveBlobsTests
{
    private readonly Mock<IMidnightContainerRetriever> _containerRetrieverMock = new();
    private readonly Fixture _fixture = new();
    private readonly Mock<IOptions<MidnightBlobsBuilder>> _optionsMock = new();

    public SaveBlobsTests()
    {
        _optionsMock.Setup(m => m.Value).Returns(new MidnightBlobsBuilder());
    }

    [Fact]
    public async Task GivenSave_IfUseContainerUsed_ShouldSaveBlobToContainer()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();
        var content = _fixture.Create<string>();

        _containerRetrieverMock.Setup(t => t.GetContainer(container))
            .ReturnsAsync(containerMock.Object);

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);

        var data = new BinaryData(content);
        var sut = new MidnightBlobUploader(_containerRetrieverMock.Object, _optionsMock.Object);
        await sut.Save(data, blobLocation, container);
        blobMock.Verify(t => t.UploadAsync(data, false, default), Times.Once);
    }

    [Fact]
    public async Task GivenSave_WhenOverwriteIsSetInSettings_ShouldSaveBlobToContainerWithOverwrite()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var blobMock = new Mock<BlobClient>();
        var blobLocation = _fixture.Create<string>();
        var container = _fixture.Create<string>();
        var content = _fixture.Create<string>();

        _containerRetrieverMock.Setup(t => t.GetContainer(container))
            .ReturnsAsync(containerMock.Object);

        containerMock.Setup(t => t.GetBlobClient(blobLocation))
            .Returns(blobMock.Object);

        var data = new BinaryData(content);

        _optionsMock.Setup(m => m.Value).Returns(new MidnightBlobsBuilder
        {
            ShouldOverwriteExistingBlob = true
        });

        var sut = new MidnightBlobUploader(_containerRetrieverMock.Object, _optionsMock.Object);
        await sut.Save(data, blobLocation, container);
        blobMock.Verify(t => t.UploadAsync(data, true, default), Times.Once);
    }
}