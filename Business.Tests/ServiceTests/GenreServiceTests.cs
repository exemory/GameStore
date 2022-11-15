using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Business.Tests.ServiceTests;

public class GenreServiceTests : TestsBase
{
    private readonly GenreService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();

    public GenreServiceTests()
    {
        _sut = new GenreService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGenres()
    {
        // Arrange
        var genres = Fixture.Build<Genre>()
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