﻿using System.Net;
using System.Net.Http.Json;
using Business.DataTransferObjects;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.ControllerTests;

public class PlatformTypesControllerTests : IntegrationTests
{
    private const string PlatformTypesUrl = "/api/platformTypes";

    public PlatformTypesControllerTests(TestingWebAppFactory appFactory) : base(appFactory)
    {
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnAllPlatformTypes()
    {
        // Arrange
        var testClient = AppFactory.CreateClient();
        
        // Act
        var response = await testClient.GetAsync(PlatformTypesUrl);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var platformTypes = await response.Content.ReadFromJsonAsync<IEnumerable<PlatformTypeDto>>();
        platformTypes.Should().NotBeEmpty();
    }
}