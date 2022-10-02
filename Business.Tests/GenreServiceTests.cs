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

public class GenreServiceTests
{
    private readonly GenreService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly Fixture _fixture = new();
    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();

    public GenreServiceTests()
    {
        _sut = new GenreService(_unitOfWorkMock.Object, _mapper);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGenres()
    {
        // Arrange
        var genres = _fixture.Build<Genre>()
            .Without(g => g.Games)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<GenreDto>>(genres);

        _unitOfWorkMock.Setup(u => u.GenreRepository.GetAllAsync())
            .ReturnsAsync(genres);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}