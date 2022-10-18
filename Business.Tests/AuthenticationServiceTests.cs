using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Business.Options;
using Business.Services;
using Data.Entities;
using ExpectedObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Business.Tests;

public class AuthenticationServiceTests
{
    private readonly AuthenticationService _sut;

    private readonly UserManager<User> _userManager =
        Substitute.For<UserManager<User>>(Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null,
            null);

    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();
    private readonly IOptions<JwtOptions> _jwtOptions = Substitute.For<IOptions<JwtOptions>>();
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    private readonly Fixture _fixture = new();

    public AuthenticationServiceTests()
    {
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var jwtOptions = _fixture.Build<JwtOptions>()
            .With(j => j.Lifetime, TimeSpan.FromDays(1))
            .Create();

        _jwtOptions.Value.Returns(jwtOptions);
        _dateTimeProvider.UtcNow.Returns(_fixture.Create<DateTimeOffset>().ToUniversalTime());

        _sut = new AuthenticationService(_userManager, _mapper, _jwtOptions, _dateTimeProvider);
    }

    [Fact]
    public async Task SignUpAsync_ShouldRegisterUser()
    {
        // Arrange
        var signUpDto = _fixture.Create<SignUpDto>();
        var expectedUserToCreate = _mapper.Map<User>(signUpDto)
            .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

        _userManager.CreateAsync(Arg.Is<User>(u => expectedUserToCreate.Equals(u)), signUpDto.Password)
            .Returns(IdentityResult.Success);

        // Act
        await _sut.SignUpAsync(signUpDto);

        // Assert
        await _userManager.Received(1)
            .CreateAsync(Arg.Is<User>(u => expectedUserToCreate.Equals(u)), signUpDto.Password);
    }

    [Fact]
    public async Task SignUpAsync_ShouldFail_WhenCreatingUserFailed()
    {
        // Arrange
        var signUpDto = _fixture.Create<SignUpDto>();
        var expectedUserToCreate = _mapper.Map<User>(signUpDto)
            .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));
        var errors = _fixture.CreateMany<IdentityError>().ToArray();
        var identityError = IdentityResult.Failed(errors);
        var expectedErrorMessage = string.Join(' ', errors.Select(e => e.Description));

        _userManager.CreateAsync(Arg.Is<User>(u => expectedUserToCreate.Equals(u)), signUpDto.Password)
            .Returns(identityError);

        // Act
        Func<Task> result = async () => await _sut.SignUpAsync(signUpDto);

        // Assert
        await result.Should().ThrowAsync<RegistrationException>()
            .WithMessage(expectedErrorMessage);

        await _userManager.Received(1)
            .CreateAsync(Arg.Is<User>(u => expectedUserToCreate.Equals(u)), signUpDto.Password);
    }

    [Theory]
    [MemberData(nameof(SignInAsync_ShouldAuthenticateUser_TestData))]
    public async Task SignInAsync_ShouldAuthenticateUser(User user, SignInDto signInDto)
    {
        // Arrange
        var expectedUser = user
            .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));
        var userRoles = _fixture.CreateMany<string>().ToList();
        var expectedTokenExpiration = (_dateTimeProvider.UtcNow + _jwtOptions.Value.Lifetime)
            .ToUnixTimeSeconds()
            .ToString();
        var expectedUserInfo = _mapper.Map<User, UserInfoDto>(user);
        expectedUserInfo.UserRoles = userRoles;

        _userManager.FindByNameAsync(user.UserName).Returns(user);
        _userManager.FindByEmailAsync(user.Email).Returns(user);
        _userManager.CheckPasswordAsync(Arg.Is<User>(u => expectedUser.Equals(u)), signInDto.Password)
            .Returns(true);
        _userManager.GetRolesAsync(Arg.Is<User>(u => expectedUser.Equals(u))).Returns(userRoles);

        // Act
        var result = await _sut.SignInAsync(signInDto);

        // Assert
        result.UserInfo.Should().BeEquivalentTo(expectedUserInfo);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);

        jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Iss).Value
            .Should().Be(_jwtOptions.Value.Issuer);
        jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Aud).Value
            .Should().Be(_jwtOptions.Value.Audience);
        jwt.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value
            .Should().Be(user.Id.ToString());
        jwt.Claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .Should().BeEquivalentTo(userRoles);
        jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value
            .Should().Be(expectedTokenExpiration);

        await _userManager.Received(1)
            .CheckPasswordAsync(Arg.Is<User>(u => expectedUser.Equals(u)), signInDto.Password);
    }

    public static IEnumerable<object[]> SignInAsync_ShouldAuthenticateUser_TestData()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var user1 = fixture.Create<User>();
        yield return new object[]
        {
            user1,
            fixture.Build<SignInDto>()
                .With(d => d.Login, user1.UserName)
                .Create()
        };

        var user2 = fixture.Create<User>();
        yield return new object[]
        {
            user2,
            fixture.Build<SignInDto>()
                .With(d => d.Login, user2.Email)
                .Create()
        };
    }

    [Fact]
    public async Task SignInAsync_ShouldFail_WhenUserDoesNotExist()
    {
        // Arrange
        var signInDto = _fixture.Create<SignInDto>();

        _userManager.FindByNameAsync(signInDto.Login).ReturnsNull();
        _userManager.FindByEmailAsync(signInDto.Login).ReturnsNull();

        // Act
        Func<Task> result = async () => await _sut.SignInAsync(signInDto);

        // Assert
        await result.Should().ThrowAsync<AuthenticationException>();

        await _userManager.DidNotReceive().CheckPasswordAsync(Arg.Any<User>(), Arg.Any<string>());
    }

    [Fact]
    public async Task SignInAsync_ShouldFail_WhenPasswordIsInvalid()
    {
        // Arrange
        var signInDto = _fixture.Create<SignInDto>();
        var user = _fixture.Create<User>();
        var expectedUserToCheckPassword = user
            .ToExpectedObject(o => o.Ignore(u => u.ConcurrencyStamp));

        _userManager.FindByNameAsync(signInDto.Login)
            .Returns(user);
        _userManager.FindByEmailAsync(signInDto.Login).ReturnsNull();
        _userManager.CheckPasswordAsync(Arg.Is<User>(u => expectedUserToCheckPassword.Equals(u)), signInDto.Password)
            .Returns(false);

        // Act
        Func<Task> result = async () => await _sut.SignInAsync(signInDto);

        // Assert
        await result.Should().ThrowAsync<AuthenticationException>();

        await _userManager.Received(1)
            .CheckPasswordAsync(Arg.Is<User>(u => expectedUserToCheckPassword.Equals(u)), signInDto.Password);
    }
}