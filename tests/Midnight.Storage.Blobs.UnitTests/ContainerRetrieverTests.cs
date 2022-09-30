#region

using AutoFixture;
using Azure.Storage.Blobs;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Midnight.Storage.Blobs.Extensions;
using Midnight.Storage.Blobs.Internal;
using Moq;

#endregion

namespace Midnight.Storage.Blobs.UnitTests;

public class ContainerRetrieverTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock = new();
    private readonly Mock<IMidnightContainerService> _containerServiceMock = new();
    private readonly Fixture _fixture = new();
    private readonly Mock<IOptions<MidnightBlobsBuilder>> _optionsMock = new();

    public ContainerRetrieverTests()
    {
        _containerServiceMock.Setup(t => t.GetServiceClient()).Returns(_blobServiceClientMock.Object);
        _optionsMock.Setup(m => m.Value).Returns(new MidnightBlobsBuilder());
    }

    [Fact]
    public async Task GivenGet_WhereUseContainerIsNotUsed_ShouldThrowNullReferenceException()
    {
        var sut = new MidnightContainerRetriever(_containerServiceMock.Object, _optionsMock.Object);
        await sut.Invoking(t => t.GetContainer(null))
            .Should()
            .ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task GivenGet_WhenOptionSpecifiesShouldEnsureCreated_ShouldCallCreateIfNotExists()
    {
        var containerMock = new Mock<BlobContainerClient>();
        var container = _fixture.Create<string>();

        _blobServiceClientMock.Setup(t => t.GetBlobContainerClient(container))
            .Returns(containerMock.Object);
        ;
        ;
        _optionsMock.Setup(m => m.Value).Returns(new MidnightBlobsBuilder
        {
            ShouldCreateContainerIfNotExisting = true
        });
        var sut = new MidnightContainerRetriever(_containerServiceMock.Object, _optionsMock.Object);
        await sut.GetContainer(container);
        containerMock.Verify(t => t.CreateIfNotExistsAsync(default, null, null, default), Times.Once);
    }
}