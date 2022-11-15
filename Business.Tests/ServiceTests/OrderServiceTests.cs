using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Services;
using Data.Entities;
using Data.Enums;
using Data.Interfaces;
using ExpectedObjects;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Business.Tests.ServiceTests;

public class OrderServiceTests : TestsBase
{
    private readonly OrderService _sut;

    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();

    public OrderServiceTests()
    {
        _sut = new OrderService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateOrder()
    {
        // Arrange
        var orderCreationDto = Fixture.Build<OrderCreationDto>()
            .With(o => o.PaymentType, PaymentType.Card.ToString)
            .Create();
        var games = orderCreationDto.Items
            .Select(i => Fixture.Build<Game>().With(g => g.Id, i.GameId).Create())
            .ToList();
        var expectedGameIds = games.Select(g => g.Id).ToList().ToExpectedObject();
        var expectedOrderToAdd = _mapper.Map<Order>(orderCreationDto).ToExpectedObject();

        _unitOfWork.GameRepository.GetByIds(Arg.Is<IEnumerable<Guid>>(ids => expectedGameIds.Equals(ids)))
            .Returns(games);

        // Act
        await _sut.CreateAsync(orderCreationDto);

        // Assert
        _unitOfWork.OrderRepository.Received().Add(Arg.Is<Order>(o => expectedOrderToAdd.Equals(o)));
        await _unitOfWork.Received().SaveAsync();
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenGamesDoesNotExists()
    {
        // Arrange
        var orderCreationDto = Fixture.Create<OrderCreationDto>();

        _unitOfWork.GameRepository.GetByIds(Arg.Any<IEnumerable<Guid>>()).Returns(new List<Game>());

        // Act
        var result = () => _sut.CreateAsync(orderCreationDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>();

        _unitOfWork.OrderRepository.DidNotReceive().Add(Arg.Any<Order>());
        await _unitOfWork.DidNotReceive().SaveAsync();
    }
}