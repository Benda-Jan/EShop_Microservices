﻿using System.Text.Json;
using Cart.API.EventsHandling;
using Cart.API.Payment;
using Cart.Entities.DbSet;
using Cart.Entities.Dtos;
using Cart.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Cart.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class CartController(ICartRepository cartRepository, IPaymentClient paymentClient, EventBusCartItemUpdated eventBusPublisher) : ControllerBase
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IPaymentClient _paymentClient = paymentClient;
    private readonly EventBusCartItemUpdated _eventBusPublisher = eventBusPublisher;

    [HttpGet]
    //[Authorize]
    [Route("user/{userId}/items")]
    public Task<IActionResult> GetItems(string userId)
        => HandleAction(async () => await _cartRepository.GetCartItems(userId));

    [HttpPost]
    //[Authorize]
    [Route("user/{userId}/item")]
    public Task<IActionResult> CreateItem(string userId, [FromBody]CartItemInputDto cartItem)
        => HandleAction(async () => 
        {
            var result = await _cartRepository.InsertCartItem(userId, cartItem);
            _eventBusPublisher.Publish(JsonSerializer.Serialize(new CartItemSerializer {CatalogItemId = cartItem.CatalogItemId, Quantity = cartItem.Quantity}));
            return result;
        });

    [HttpPost]
    //[Authorize]
    [Route("user/{userId}/ProcessPayment")]
    public Task<IActionResult> ProcessPayment(string userId)
        => HandleAction(async () => await _paymentClient.SendPayment(userId));

    [HttpPut]
    //[Authorize]
    [Route("user/{userId}/item")]
    public Task<IActionResult> UpdateItem(string userId, string cartItemId, [FromBody]int quantity)
        => HandleAction(async () => await _cartRepository.UpdateCartItem(userId, cartItemId, quantity));

    [HttpDelete]
    //[Authorize]
    [Route("user/{userId}/item/{cartItemId}")]
    public Task<IActionResult> DeleteItem(string userId, string cartItemId)
        => HandleAction(async () => 
        {
            var result = await _cartRepository.DeleteCartItem(userId, cartItemId);
            if (result != null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(result));
            return result;
        });

    private async Task<IActionResult> HandleAction(Func<Task<Object?>> func)
    {
        try
        {
            var result = await func();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

