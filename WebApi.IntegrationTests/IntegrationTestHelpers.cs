using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using AutoFixture;
using Business;
using Business.DataTransferObjects;

namespace WebApi.IntegrationTests;

public static class IntegrationTestHelpers
{
    private static readonly Fixture Fixture = new();

    public static SignUpDto CreateSignUpDto()
    {
        return Fixture.Build<SignUpDto>()
            .With(d => d.Username, Fixture.Create<string>()[..10])
            .With(d => d.Email, Fixture.Create<MailAddress>().ToString())
            .Create();
    }

    public static async Task<SignInDto> AuthorizeAsAdminAsync(HttpClient httpClient)
    {
        return await AuthorizeAsync(httpClient, RequiredData.Admin.UserName, RequiredData.AdminPassword);
    }

    public static void RemoveAuthorization(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public static async Task<SignInDto> AuthorizeAsUserAsync(HttpClient httpClient)
    {
        var signUpDto = CreateSignUpDto();
        var response = await httpClient.PostAsJsonAsync("api/auth/sign-up", signUpDto);

        response.EnsureSuccessStatusCode();

        return await AuthorizeAsync(httpClient, signUpDto.Username, signUpDto.Password);
    }

    private static async Task<SignInDto> AuthorizeAsync(HttpClient httpClient, string login, string password)
    {
        RemoveAuthorization(httpClient);

        var signInDto = Fixture.Build<SignInDto>()
            .With(d => d.Login, login)
            .With(d => d.Password, password)
            .Create();

        var response = await httpClient.PostAsJsonAsync("api/auth/sign-in", signInDto);
        response.EnsureSuccessStatusCode();

        var session = (await response.Content.ReadFromJsonAsync<SessionDto>())!;
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", $"{session.AccessToken}");

        return signInDto;
    }
}