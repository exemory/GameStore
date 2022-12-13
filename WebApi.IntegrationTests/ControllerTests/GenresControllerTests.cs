using System.Net;
using System.Net.Http.Json;
using Business.DataTransferObjects;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.ControllerTests;

public class GenresControllerTests : IntegrationTests
{
    private const string GenresUrl = "/api/genres";
    
    public GenresControllerTests(TestingWebAppFactory appFactory) : base(appFactory)
    {
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnAllGenres()
    { 
        // Arrange

        // Act
        var response = await TestClient.GetAsync(GenresUrl);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var genres = await response.Content.ReadFromJsonAsync<IEnumerable<GenreDto>>();
        genres.Should().NotBeEmpty();
    }
}