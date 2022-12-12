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

    public static async Task AuthorizeAsAdminAsync(HttpClient httpClient)
    {
        await AuthorizeAsync(httpClient, RequiredData.Admin.UserName, RequiredData.AdminPassword);
    }

    public static void RemoveAuthorization(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public static async Task AuthorizeAsUserAsync(HttpClient httpClient)
    {
        var signUpDto = CreateSignUpDto();
        var response = await httpClient.PostAsJsonAsync("api/auth/sign-up", signUpDto);

        response.EnsureSuccessStatusCode();

        await AuthorizeAsync(httpClient, signUpDto.Username, signUpDto.Password);
    }

    private static async Task AuthorizeAsync(HttpClient httpClient, string login, string password)
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
    }

    public static async Task<GameWithGenresDto> CreateGameAsync(HttpClient httpClient)
    {
        const string url = "api/games";

        var imageUploadResult = await UploadImageAsync(httpClient);

        var gameCreationDto = Fixture.Build<GameCreationDto>()
            .With(d => d.Key, Fixture.Create<string>().Substring(0, 20))
            .With(d => d.ImageFileName, imageUploadResult.ImageFileName)
            .Without(d => d.GenreIds)
            .Without(d => d.PlatformTypeIds)
            .Create();

        var response = await httpClient.PostAsJsonAsync(url, gameCreationDto);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<GameWithGenresDto>())!;
    }

    public static async Task<ImageUploadResultDto> UploadImageAsync(HttpClient httpClient)
    {
        const string url = "api/games/images";
        const string contentName = "file";
        const string imageName = "test.jpg";
        var imageStream = new MemoryStream();

        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(imageStream), contentName, imageName);

        var response = await httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ImageUploadResultDto>())!;
    }

    public static async Task<CommentDto> CreateCommentAsync(HttpClient httpClient, Guid gameId)
    {
        const string commentsUrl = "api/comments";
        var commentCreationDto = Fixture.Build<CommentCreationDto>()
            .With(d => d.GameId, gameId)
            .Without(d => d.ParentId)
            .Create();

        var response = await httpClient.PostAsJsonAsync(commentsUrl, commentCreationDto);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<CommentDto>())!;
    }

    public static async Task<CommentDto> CreateDeletedComment(HttpClient httpClient, Guid gameId)
    {
        var comment = await CreateCommentAsync(httpClient, gameId);
        var url = $"api/comments/{comment.Id}";

        var response = await httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();

        return comment;
    }
}