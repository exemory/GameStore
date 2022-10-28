﻿using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using ExpectedObjects;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace Business.Tests;

public class CommentServiceTests
{
    private readonly CommentService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly Fixture _fixture = new();
    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();
    private readonly Mock<ISession> _sessionMock = new();

    public CommentServiceTests()
    {
        _sut = new CommentService(_unitOfWorkMock.Object, _mapper, _sessionMock.Object);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateGame_WhenParentCommentUnspecified()
    {
        // Arrange
        var commentCreationDto = _fixture.Build<CommentCreationDto>()
            .Without(c => c.ParentId)
            .Create();
        var userId = _fixture.Create<Guid>();
        var mappedComment = _mapper.Map<Comment>(commentCreationDto);
        mappedComment.UserId = userId;
        var expectedToCreate = mappedComment.ToExpectedObject();
        var expected = _mapper.Map<CommentDto>(mappedComment);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(commentCreationDto.GameId))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.CommentRepository.Add(It.IsAny<Comment>()));
        _sessionMock.Setup(s => s.UserId).Returns(userId);

        // Act
        var result = await _sut.CreateAsync(commentCreationDto);

        // Assert
        result.Should().BeEquivalentTo(expected);

        _unitOfWorkMock.Verify(u => u.CommentRepository.Add(It.Is<Comment>(c => expectedToCreate.Equals(c))),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateGame_WhenParentCommentSpecified()
    {
        // Arrange
        var commentCreationDto = _fixture.Create<CommentCreationDto>();
        var parentComment = _fixture.Build<Comment>()
            .With(c => c.GameId, commentCreationDto.GameId)
            .Create();
        var userId = _fixture.Create<Guid>();
        var mappedComment = _mapper.Map<Comment>(commentCreationDto);
        mappedComment.UserId = userId;
        var expectedToCreate = mappedComment.ToExpectedObject();
        var expected = _mapper.Map<CommentDto>(mappedComment);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(commentCreationDto.GameId))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(commentCreationDto.ParentId!.Value))
            .ReturnsAsync(parentComment);
        _sessionMock.Setup(s => s.UserId).Returns(userId);

        // Act
        var result = await _sut.CreateAsync(commentCreationDto);

        // Assert
        result.Should().BeEquivalentTo(expected);

        _unitOfWorkMock.Verify(u => u.CommentRepository.Add(It.Is<Comment>(c => expectedToCreate.Equals(c))),
            Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenGameDoesNotExists()
    {
        // Arrange
        var commentCreationDto = _fixture.Create<CommentCreationDto>();
        var expectedExceptionMessage = $"Game with id '{commentCreationDto.GameId}' not found.";

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(commentCreationDto.GameId))
            .ReturnsAsync((Game?) null);

        // Act
        Func<Task> result = async () => await _sut.CreateAsync(commentCreationDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);

        _unitOfWorkMock.Verify(u => u.CommentRepository.Add(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenParentCommentDoesNotExists()
    {
        // Arrange
        var commentCreationDto = _fixture.Create<CommentCreationDto>();
        var expectedExceptionMessage = $"Comment with id '{commentCreationDto.ParentId}' not found.";

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(commentCreationDto.GameId))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(commentCreationDto.ParentId!.Value))
            .ReturnsAsync((Comment?) null);

        // Act
        Func<Task> result = async () => await _sut.CreateAsync(commentCreationDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);

        _unitOfWorkMock.Verify(u => u.CommentRepository.Add(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenParentCommentFromOtherGame()
    {
        // Arrange
        var commentCreationDto = _fixture.Create<CommentCreationDto>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(commentCreationDto.GameId))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(commentCreationDto.ParentId!.Value))
            .ReturnsAsync(_fixture.Create<Comment>());

        // Act
        Func<Task> result = async () => await _sut.CreateAsync(commentCreationDto);

        // Assert
        await result.Should().ThrowExactlyAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u => u.CommentRepository.Add(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllByGameKeyAsync_ShouldReturnAllCommentsRelatedToGame()
    {
        // Arrange
        var game = _fixture.Create<Game>();
        var comments = _fixture.Build<Comment>()
            .Without(c => c.Game)
            .CreateMany();
        var expectedComments = _mapper.Map<IEnumerable<CommentDto>>(comments);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(game.Key))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.CommentRepository.GetAllByGameKeyAsync(game.Key))
            .ReturnsAsync(comments);

        // Act
        var result = await _sut.GetAllByGameKeyAsync(game.Key);

        // Assert
        result.Should().BeEquivalentTo(expectedComments);
    }

    [Fact]
    public async Task GetAllByGameKeyAsync_ShouldFail_WhenGameDoesNotExists()
    {
        // Arrange
        var nonexistentGameKey = _fixture.Create<string>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(nonexistentGameKey))
            .ReturnsAsync((Game?) null);

        // Act
        Func<Task> result = async () => await _sut.GetAllByGameKeyAsync(nonexistentGameKey);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>();
    }

    [Fact]
    public async Task EditAsync_ShouldEditComment()
    {
        // Arrange
        var comment = _fixture.Create<Comment>();
        var commentUpdateDto = _fixture.Create<CommentUpdateDto>();
        var expectedCommentToUpdate = _mapper.Map(commentUpdateDto, comment.DeepClone()).ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(comment.UserId);

        // Act
        await _sut.EditAsync(comment.Id, commentUpdateDto);

        // Assert
        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.Is<Comment>(c => expectedCommentToUpdate.Equals(c))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task EditAsync_ShouldFail_WhenCommentDoesNotExist()
    {
        // Arrange
        var nonexistentCommentId = _fixture.Create<Guid>();
        var commentUpdateDto = _fixture.Create<CommentUpdateDto>();
        var userId = _fixture.Create<Guid>();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(nonexistentCommentId))
            .ReturnsAsync((Comment?) null);
        _sessionMock.Setup(s => s.UserId).Returns(userId);

        // Act
        var result = () => _sut.EditAsync(nonexistentCommentId, commentUpdateDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task EditAsync_ShouldFail_WhenUserTriesToEditOtherUsersComment()
    {
        // Arrange
        var comment = _fixture.Create<Comment>();
        var commentUpdateDto = _fixture.Create<CommentUpdateDto>();
        var userId = _fixture.Create<Guid>();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(userId);

        // Act
        var result = () => _sut.EditAsync(comment.Id, commentUpdateDto);

        // Assert
        await result.Should().ThrowExactlyAsync<AccessDeniedException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkCommentAsDeleted()
    {
        // Arrange
        var comment = _fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Create();
        var updatedComment = comment.DeepClone();
        updatedComment.Deleted = true;
        var expectedCommentToUpdate = updatedComment.ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(comment.UserId);

        // Act
        await _sut.DeleteAsync(comment.Id);

        // Assert
        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.Is<Comment>(c => expectedCommentToUpdate.Equals(c))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenCommentDoesNotExist()
    {
        // Arrange
        var nonexistentCommentId = _fixture.Create<Guid>();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(nonexistentCommentId))
            .ReturnsAsync((Comment?) null);

        // Act
        var result = () => _sut.DeleteAsync(nonexistentCommentId);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenUserTriesToDeleteOtherUsersComment()
    {
        // Arrange
        var comment = _fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Create();
        var userId = _fixture.Create<Guid>();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(userId);

        // Act
        var result = () => _sut.DeleteAsync(comment.Id);

        // Assert
        await result.Should().ThrowExactlyAsync<AccessDeniedException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenCommentHasAlreadyBeenDeleted()
    {
        // Arrange
        var comment = _fixture.Build<Comment>()
            .With(c => c.Deleted, true)
            .Create();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(comment.UserId);

        // Act
        var result = () => _sut.DeleteAsync(comment.Id);

        // Assert
        await result.Should().ThrowExactlyAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task RestoreAsync_ShouldRestoreComment()
    {
        // Arrange
        var comment = _fixture.Build<Comment>()
            .With(c => c.Deleted, true)
            .Create();
        var updatedComment = comment.DeepClone();
        updatedComment.Deleted = false;
        var expectedCommentToUpdate = updatedComment.ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(comment.UserId);

        // Act
        await _sut.RestoreAsync(comment.Id);

        // Assert
        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.Is<Comment>(c => expectedCommentToUpdate.Equals(c))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task RestoreAsync_ShouldFail_WhenCommentDoesNotExist()
    {
        // Arrange
        var nonexistentCommentId = _fixture.Create<Guid>();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(nonexistentCommentId))
            .ReturnsAsync((Comment?) null);

        // Act
        var result = () => _sut.RestoreAsync(nonexistentCommentId);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task RestoreAsync_ShouldFail_WhenUserTriesToRestoreOtherUsersComment()
    {
        // Arrange
        var comment = _fixture.Build<Comment>()
            .With(c => c.Deleted, true)
            .Create();
        var userId = _fixture.Create<Guid>();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(userId);

        // Act
        var result = () => _sut.RestoreAsync(comment.Id);

        // Assert
        await result.Should().ThrowExactlyAsync<AccessDeniedException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task RestoreAsync_ShouldFail_WhenCommentIsNotDeleted()
    {
        // Arrange
        var comment = _fixture.Build<Comment>()
            .With(c => c.Deleted, false)
            .Create();

        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(comment.Id))
            .ReturnsAsync(comment);
        _sessionMock.Setup(s => s.UserId).Returns(comment.UserId);

        // Act
        var result = () => _sut.RestoreAsync(comment.Id);

        // Assert
        await result.Should().ThrowExactlyAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u =>
            u.CommentRepository.Update(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }
}