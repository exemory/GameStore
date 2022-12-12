using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using Business.DataTransferObjects;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.ControllerTests;

public class CommentsControllerTests : IntegrationTests
{
    private const string CommentsUrl = "api/comments";

    [Fact]
    public async Task New_ShouldCreateComment()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);

        var commentCreationDto = Fixture.Build<CommentCreationDto>()
            .With(d => d.GameId, game.Id)
            .Without(d => d.ParentId)
            .Create();

        // Act
        var response = await TestClient.PostAsJsonAsync(CommentsUrl, commentCreationDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);

        var comment = await response.Content.ReadFromJsonAsync<CommentDto>();
        comment.Should().BeEquivalentTo(commentCreationDto, o => o.ExcludingMissingMembers());
    }

    [Fact]
    public async Task New_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var commentCreationDto = Fixture.Build<CommentCreationDto>()
            .With(d => d.GameId, game.Id)
            .Without(d => d.ParentId)
            .Create();

        // Act
        var response = await TestClient.PostAsJsonAsync(CommentsUrl, commentCreationDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllByGameKey_ShouldReturnGameComments()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        var comment1 = await IntegrationTestHelpers.CreateCommentAsync(TestClient, game.Id);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var comment2 = await IntegrationTestHelpers.CreateCommentAsync(TestClient, game.Id);
        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var url = $"{CommentsUrl}?gameKey={game.Key}";

        // Act
        var response = await TestClient.GetAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var comments = await response.Content.ReadFromJsonAsync<IEnumerable<CommentDto>>();
        comments.Should().BeEquivalentTo(new[] {comment1, comment2});
    }

    [Fact]
    public async Task Edit_ShouldEditComment()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var comment = await IntegrationTestHelpers.CreateCommentAsync(TestClient, game.Id);

        var url = $"{CommentsUrl}/{comment.Id}";
        var updateCommentDto = Fixture.Create<CommentUpdateDto>();

        // Act
        var response = await TestClient.PutAsJsonAsync(url, updateCommentDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Edit_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var comment = await IntegrationTestHelpers.CreateCommentAsync(TestClient, game.Id);
        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var url = $"{CommentsUrl}/{comment.Id}";
        var updateCommentDto = Fixture.Create<CommentUpdateDto>();

        // Act
        var response = await TestClient.PutAsJsonAsync(url, updateCommentDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_ShouldDeleteComment()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var comment = await IntegrationTestHelpers.CreateCommentAsync(TestClient, game.Id);

        var url = $"{CommentsUrl}/{comment.Id}";

        // Act
        var response = await TestClient.DeleteAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var comment = await IntegrationTestHelpers.CreateCommentAsync(TestClient, game.Id);
        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var url = $"{CommentsUrl}/{comment.Id}";

        // Act
        var response = await TestClient.DeleteAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Restore_ShouldRestoreComment()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var comment = await IntegrationTestHelpers.CreateDeletedComment(TestClient, game.Id);

        var url = $"{CommentsUrl}/{comment.Id}/restore";

        // Act
        var response = await TestClient.PutAsync(url, null);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Restore_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        await IntegrationTestHelpers.AuthorizeAsAdminAsync(TestClient);
        var game = await IntegrationTestHelpers.CreateGameAsync(TestClient);
        await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var comment = await IntegrationTestHelpers.CreateDeletedComment(TestClient, game.Id);
        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var url = $"{CommentsUrl}/{comment.Id}/restore";

        // Act
        var response = await TestClient.PutAsync(url, null);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
    }
}