﻿using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using Business;
using Business.DataTransferObjects;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.ControllerTests;

public class AuthenticationControllerTests : IntegrationTests
{
    private const string SignUpUrl = "api/auth/sign-up";
    private const string SignInUrl = "api/auth/sign-in";

    public AuthenticationControllerTests(TestingWebAppFactory appFactory) : base(appFactory)
    {
    }
    
    [Fact]
    public async Task SignUp_ShouldRegisterUser()
    {
        // Arrange
        var testClient = AppFactory.CreateClient();
        var signUpDto = IntegrationTestHelpers.CreateSignUpDto();

        // Act
        var response = await testClient.PostAsJsonAsync(SignUpUrl, signUpDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);
        
        // Cleanup
        DbContext.Users.Remove(DbContext.Users.First(u => u.UserName == signUpDto.Username));
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task SignIn_ShouldLoginUser()
    {
        // Arrange
        var testClient = AppFactory.CreateClient();
        var signUpDto = IntegrationTestHelpers.CreateSignUpDto();
        var signInDto = Fixture.Build<SignInDto>()
            .With(d => d.Login, signUpDto.Username)
            .With(d => d.Password, signUpDto.Password)
            .Create();

        var signUpResponse = await testClient.PostAsJsonAsync(SignUpUrl, signUpDto);
        signUpResponse.EnsureSuccessStatusCode();

        // Act
        var signInResponse = await testClient.PostAsJsonAsync(SignInUrl, signInDto);

        // Assert
        signInResponse.Should().HaveStatusCode(HttpStatusCode.OK);

        var session = await signInResponse.Content.ReadFromJsonAsync<SessionDto>();
        session.Should().NotBeNull();
        session!.AccessToken.Should().NotBeEmpty();
        session.UserInfo.Should().BeEquivalentTo(signUpDto, o => o.ExcludingMissingMembers());
        session.UserInfo.Roles.Should().BeEmpty();
        session.UserInfo.HasAvatar.Should().BeFalse();
        
        // Cleanup
        DbContext.Users.Remove(DbContext.Users.First(u => u.UserName == signUpDto.Username));
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task SignIn_ShouldLoginAdminUser()
    {
        // Arrange
        var testClient = AppFactory.CreateClient();
        const string adminRole = "Admin";
        var signInDto = Fixture.Build<SignInDto>()
            .With(d => d.Login, RequiredData.Admin.UserName)
            .With(d => d.Password, RequiredData.AdminPassword)
            .Create();

        // Act
        var response = await testClient.PostAsJsonAsync(SignInUrl, signInDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var session = await response.Content.ReadFromJsonAsync<SessionDto>();
        session.Should().NotBeNull();
        session!.AccessToken.Should().NotBeEmpty();
        session.UserInfo.Should().BeEquivalentTo(RequiredData.Admin, o => o.ExcludingMissingMembers().Excluding(u => u.Id));
        session.UserInfo.Roles.Should().BeEquivalentTo(adminRole);
        session.UserInfo.HasAvatar.Should().BeFalse();
    }
}