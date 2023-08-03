using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShubT.Services.ShoppingCartAPI.Data;
using ShubT.Services.ShoppingCartAPI.DTOs;
using ShubT.Services.ShoppingCartAPI.Models;
using ShubT.Services.ShoppingCartAPI.Service.Interfaces;

namespace ShubT.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private ResponseDTO _responseDTO;

        public CartAPIController(AppDbContext context, IMapper mapper, IProductService productService, ICouponService couponService)
        {
            _context = context;
            _mapper = mapper;
            _responseDTO = new ResponseDTO();
            _productService = productService;
            _couponService = couponService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cartDTO = new CartDTO();

                var cartHeader = _context.CartHeaders.First(u => u.UserId == userId);
                cartDTO.CartHeaderDTO = _mapper.Map<CartHeaderDTO>(cartHeader);

                var cartDetails = _context.CartDetails.Where(u => u.CartHeaderId == cartDTO.CartHeaderDTO.Id);
                cartDTO.CartDetailsDTO = _mapper.Map<IEnumerable<CartDetailsDTO>>(cartDetails);

                var productList = await _productService.GetAllProductsAsync();

                foreach (var item in cartDTO.CartDetailsDTO)
                {
                    item.ProductDTO = productList.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cartDTO.CartHeaderDTO.CartTotal += item.Count * item.ProductDTO.Price;
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cartDTO.CartHeaderDTO.CouponCode))
                {
                    var coupon = await _couponService.GetCouponsByCodeAsync(cartDTO.CartHeaderDTO.CouponCode);
                    if (coupon != null && cartDTO.CartHeaderDTO.CartTotal > coupon.MinAmount)
                    {
                        cartDTO.CartHeaderDTO.CartTotal -= coupon.DiscountAmount;
                        cartDTO.CartHeaderDTO.DiscountTotal = coupon.DiscountAmount;
                    }
                }

                _responseDTO.Result = cartDTO;
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDTO> ApplyCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartFromDB = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDTO.CartHeaderDTO.UserId);
                cartFromDB.CouponCode = cartDTO.CartHeaderDTO.CouponCode;
                _context.CartHeaders.Update(cartFromDB);
                await _context.SaveChangesAsync();
                _responseDTO.Result = true;
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDTO> RemoveCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartFromDB = await _context.CartHeaders.FirstAsync(u => u.UserId == cartDTO.CartHeaderDTO.UserId);
                cartFromDB.CouponCode = string.Empty;
                _context.CartHeaders.Update(cartFromDB);
                await _context.SaveChangesAsync();
                _responseDTO.Result = true;
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeadersFromDB = await _context.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeaderDTO.UserId);
                if (cartHeadersFromDB == null)
                {
                    // create cart header & details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeaderDTO);
                    _context.CartHeaders.Add(cartHeader);
                    await _context.SaveChangesAsync();

                    cartDTO.CartDetailsDTO.First().CartHeaderId = cartHeader.Id;
                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetailsDTO.First()));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //if header not null
                    var cartDetailsFromDB = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDTO.CartDetailsDTO.First().ProductId &&
                        u.CartHeaderId == cartHeadersFromDB.Id);

                    if (cartDetailsFromDB == null)
                    {
                        // create cart details
                        cartDTO.CartDetailsDTO.First().CartHeaderId = cartHeadersFromDB.Id;
                        _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetailsDTO.First()));
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        //update cart details
                        cartDTO.CartDetailsDTO.First().Count += cartDetailsFromDB.Count;
                        cartDTO.CartDetailsDTO.First().CartHeaderId = cartDetailsFromDB.CartHeaderId;
                        cartDTO.CartDetailsDTO.First().CartDetailsId = cartDetailsFromDB.CartDetailsId;
                        _context.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetailsDTO.First()));
                        await _context.SaveChangesAsync();
                    }
                }
                _responseDTO.Result = cartDTO;
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }

            return _responseDTO;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetails = _context.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
                _context.CartDetails.Remove(cartDetails);
                int totalCountOfCartItems = _context.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();

                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(u => u.Id == cartDetails.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _context.SaveChangesAsync();
                _responseDTO.Result = true;
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }

            return _responseDTO;
        }
    }
}
