using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Business.Tests;

public class PlatformTypeServiceTests
{
    private readonly PlatformTypeService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly Fixture _fixture = new();
    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();

    public PlatformTypeServiceTests()
    {
        _sut = new PlatformTypeService(_unitOfWorkMock.Object, _mapper);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPlatformTypes()
    {
        // Arrange
        var platformTypes = _fixture.Build<PlatformType>()
            .Without(p => p.Games)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<PlatformTypeDto>>(platformTypes);

        _unitOfWorkMock.Setup(u => u.PlatformTypeRepository.GetAllAsync())
            .ReturnsAsync(platformTypes);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}