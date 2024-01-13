using Microsoft.AspNetCore.Mvc;
using Tiba.OME.Application.Contracts.Commands;
using Tiba.OME.Application.Contracts.Services;

namespace Tiba.OME.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderBooksController(IOrderBookService orderBookService) : ControllerBase
{
    [HttpPost("add-order")]
    public async Task AddOrderAsync(AddOrderCommand command)
    {
        await orderBookService.AddOrder(command);
    }

    [HttpPost("update-order")]
    public async Task UpdateOrderAsync(UpdateOrderCommand command)
    {
        await orderBookService.UpdateOrder(command);
    }

    [HttpPost("cancel-order")]
    public async Task CancelOrder(CancelOrderCommand command)
    {
        await orderBookService.CancelOrder(command);
    }
}