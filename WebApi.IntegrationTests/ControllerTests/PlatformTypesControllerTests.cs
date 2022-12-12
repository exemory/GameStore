using System.Net;
using System.Net.Http.Json;
using Business.DataTransferObjects;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.ControllerTests;

public class PlatformTypesControllerTests : IntegrationTests
{
    private const string PlatformTypesUrl = "/api/platformTypes";

    [Fact]
    public async Task GetAll_ShouldReturnAllPlatformTypes()
    {
        // Arrange

        // Act
        var response = await TestClient.GetAsync(PlatformTypesUrl);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var platformTypes = await response.Content.ReadFromJsonAsync<IEnumerable<PlatformTypeDto>>();
        platformTypes.Should().NotBeEmpty();
    }
}