﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShubT.Web.Models;
using ShubT.Web.Models.Orders;
using ShubT.Web.Models.ShoppingCart;
using ShubT.Web.Models.Stripe;
using ShubT.Web.Services.Interfaces;
using ShubT.Web.Utils;
using System.IdentityModel.Tokens.Jwt;

namespace ShubT.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDTOBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDTOBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDto)
        {

            CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();
            cart.CartHeaderDTO.Phone = cartDto.CartHeaderDTO.Phone;
            cart.CartHeaderDTO.Email = cartDto.CartHeaderDTO.Email;
            cart.CartHeaderDTO.Name = cartDto.CartHeaderDTO.Name;

            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDTO stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDTO.OrderHeaderId,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeaderDTO = orderHeaderDTO
                };

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                StripeRequestDTO stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDTO>
                                            (Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                return new StatusCodeResult(303);
            }

            return View(cart);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO response = await _orderService.ValidateStripeSession(orderId);
            if (response != null & response.IsSuccess)
            {

                OrderHeaderDTO orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if (orderHeader.Status == MiscUtils.Status_Approved)
                {
                    return View(orderId);
                }
            }
            //redirect to some error page based on status
            return View(orderId);
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDto)
        {

            ResponseDTO response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDTOBasedOnLoggedInUser();
            cart.CartHeaderDTO.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDTO response = await _cartService.EmailCart(cart);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDto)
        {
            cartDto.CartHeaderDTO.CouponCode = "";
            ResponseDTO response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }


        private async Task<CartDTO> LoadCartDTOBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO response = await _cartService.GetCartByUserIdAsnyc(userId);
            if (response != null & response.IsSuccess)
            {
                CartDTO cartDto = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDTO();
        }
    }
}