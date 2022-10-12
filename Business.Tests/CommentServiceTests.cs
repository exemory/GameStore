﻿using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using ExpectedObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Business.Tests;

public class CommentServiceTests
{
    private readonly CommentService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly Fixture _fixture = new();
    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();

    public CommentServiceTests()
    {
        _sut = new CommentService(_unitOfWorkMock.Object, _mapper);
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
        var mappedComment = _mapper.Map<Comment>(commentCreationDto);
        var expectedToCreate = mappedComment.ToExpectedObject();
        var expected = _mapper.Map<CommentDto>(mappedComment);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(commentCreationDto.GameId))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.CommentRepository.Add(It.IsAny<Comment>()));

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
        var mappedComment = _mapper.Map<Comment>(commentCreationDto);
        var expectedToCreate = mappedComment.ToExpectedObject();
        var expected = _mapper.Map<CommentDto>(mappedComment);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(commentCreationDto.GameId))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.CommentRepository.GetByIdAsync(commentCreationDto.ParentId!.Value))
            .ReturnsAsync(parentComment);

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
}