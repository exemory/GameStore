using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Orders controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Constructor for initializing a <see cref="OrdersController"/> class instance
    /// </summary>
    /// <param name="orderService">Order service</param>
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create an order
    /// </summary>
    /// <param name="orderCreationDto">Order creation data</param>
    /// <response code="201">Order has been created</response>
    /// <response code="404">Some games do not exist</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Create(OrderCreationDto orderCreationDto)
    {
        await _orderService.CreateAsync(orderCreationDto);
        return CreatedAtAction(null, null);
    }
}