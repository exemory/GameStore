using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using Business.DataTransferObjects;
using Data.Entities;
using FluentAssertions;
using Xunit;

namespace WebApi.IntegrationTests.ControllerTests;

public class CommentsControllerTests : IntegrationTests
{
    private const string CommentsUrl = "api/comments";

    public CommentsControllerTests(TestingWebAppFactory appFactory) : base(appFactory)
    {
    }

    [Fact]
    public async Task New_ShouldCreateComment()
    {
        // Arrange
        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();

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

        // Cleanup
        DbContext.Games.Remove(game);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task New_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();

        var commentCreationDto = Fixture.Build<CommentCreationDto>()
            .With(d => d.GameId, game.Id)
            .Without(d => d.ParentId)
            .Create();

        // Act
        var response = await TestClient.PostAsJsonAsync(CommentsUrl, commentCreationDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

        // Cleanup
        DbContext.Games.Remove(game);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllByGameKey_ShouldReturnGameComments()
    {
        var userCredentials = await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var user = DbContext.Users.First(u => u.UserName == userCredentials.Login);

        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();

        var comments = Fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Without(c => c.ParentId)
            .Without(c => c.Parent)
            .With(c => c.GameId, game.Id)
            .With(c => c.Game, game)
            .With(c => c.UserId, user.Id)
            .With(c => c.User, user)
            .CreateMany()
            .ToList();

        DbContext.Games.Add(game);
        DbContext.Comments.AddRange(comments);
        await DbContext.SaveChangesAsync();

        var url = $"{CommentsUrl}?gameKey={game.Key}";

        // Act
        var response = await TestClient.GetAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CommentDto>>();

        result.Should().BeEquivalentTo(comments, o => o.ExcludingMissingMembers());

        // Cleanup
        DbContext.Games.Remove(game);
        DbContext.Comments.RemoveRange(comments);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Edit_ShouldEditComment()
    {
        // Arrange
        var userCredentials = await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var user = DbContext.Users.First(u => u.UserName == userCredentials.Login);

        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();

        var comment = Fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Without(c => c.ParentId)
            .Without(c => c.Parent)
            .With(c => c.GameId, game.Id)
            .With(c => c.Game, game)
            .With(c => c.UserId, user.Id)
            .With(c => c.User, user)
            .Create();

        DbContext.Games.Add(game);
        DbContext.Comments.AddRange(comment);
        await DbContext.SaveChangesAsync();

        var url = $"{CommentsUrl}/{comment.Id}";
        var updateCommentDto = Fixture.Create<CommentUpdateDto>();

        // Act
        var response = await TestClient.PutAsJsonAsync(url, updateCommentDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        // Cleanup
        DbContext.Games.Remove(game);
        DbContext.Comments.Remove(comment);
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Edit_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        var userCredentials = await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var user = DbContext.Users.First(u => u.UserName == userCredentials.Login);

        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();

        var comment = Fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Without(c => c.ParentId)
            .Without(c => c.Parent)
            .With(c => c.GameId, game.Id)
            .With(c => c.Game, game)
            .With(c => c.UserId, user.Id)
            .With(c => c.User, user)
            .Create();

        DbContext.Games.Add(game);
        DbContext.Comments.AddRange(comment);
        await DbContext.SaveChangesAsync();

        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var url = $"{CommentsUrl}/{comment.Id}";
        var updateCommentDto = Fixture.Create<CommentUpdateDto>();

        // Act
        var response = await TestClient.PutAsJsonAsync(url, updateCommentDto);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

        // Cleanup
        DbContext.Games.Remove(game);
        DbContext.Comments.Remove(comment);
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_ShouldDeleteComment()
    {
        // Arrange
        var userCredentials = await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var user = DbContext.Users.First(u => u.UserName == userCredentials.Login);

        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();

        var comment = Fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Without(c => c.ParentId)
            .Without(c => c.Parent)
            .With(c => c.GameId, game.Id)
            .With(c => c.Game, game)
            .With(c => c.UserId, user.Id)
            .With(c => c.User, user)
            .Create();

        DbContext.Games.Add(game);
        DbContext.Comments.AddRange(comment);
        await DbContext.SaveChangesAsync();

        var url = $"{CommentsUrl}/{comment.Id}";

        // Act
        var response = await TestClient.DeleteAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        // Cleanup
        DbContext.Games.Remove(game);
        DbContext.Comments.Remove(comment);
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Delete_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        var userCredentials = await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var user = DbContext.Users.First(u => u.UserName == userCredentials.Login);

        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();

        var comment = Fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Without(c => c.ParentId)
            .Without(c => c.Parent)
            .With(c => c.GameId, game.Id)
            .With(c => c.Game, game)
            .With(c => c.UserId, user.Id)
            .With(c => c.User, user)
            .Create();

        DbContext.Games.Add(game);
        DbContext.Comments.AddRange(comment);
        await DbContext.SaveChangesAsync();

        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var url = $"{CommentsUrl}/{comment.Id}";

        // Act
        var response = await TestClient.DeleteAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

        // Cleanup
        DbContext.Games.Remove(game);
        DbContext.Comments.Remove(comment);
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Restore_ShouldRestoreComment()
    {
        // Arrange
        var userCredentials = await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var user = DbContext.Users.First(u => u.UserName == userCredentials.Login);

        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();

        var comment = Fixture.Build<Comment>()
            .With(c => c.Deleted, true)
            .Without(c => c.ParentId)
            .Without(c => c.Parent)
            .With(c => c.GameId, game.Id)
            .With(c => c.Game, game)
            .With(c => c.UserId, user.Id)
            .With(c => c.User, user)
            .Create();

        DbContext.Games.Add(game);
        DbContext.Comments.AddRange(comment);
        await DbContext.SaveChangesAsync();

        var url = $"{CommentsUrl}/{comment.Id}/restore";

        // Act
        var response = await TestClient.PutAsync(url, null);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        // Cleanup
        DbContext.Games.Remove(game);
        DbContext.Comments.Remove(comment);
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Restore_ShouldFail_WhenUserIsNotAuthorized()
    {
        // Arrange
        var userCredentials = await IntegrationTestHelpers.AuthorizeAsUserAsync(TestClient);
        var user = DbContext.Users.First(u => u.UserName == userCredentials.Login);

        var game = Fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();

        var comment = Fixture.Build<Comment>()
            .With(c => c.Deleted, true)
            .Without(c => c.ParentId)
            .Without(c => c.Parent)
            .With(c => c.GameId, game.Id)
            .With(c => c.Game, game)
            .With(c => c.UserId, user.Id)
            .With(c => c.User, user)
            .Create();

        DbContext.Games.Add(game);
        DbContext.Comments.AddRange(comment);
        await DbContext.SaveChangesAsync();

        IntegrationTestHelpers.RemoveAuthorization(TestClient);

        var url = $"{CommentsUrl}/{comment.Id}/restore";

        // Act
        var response = await TestClient.PutAsync(url, null);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);

        // Cleanup
        DbContext.Games.Remove(game);
        DbContext.Comments.Remove(comment);
        DbContext.Users.Remove(user);
        await DbContext.SaveChangesAsync();
    }
}