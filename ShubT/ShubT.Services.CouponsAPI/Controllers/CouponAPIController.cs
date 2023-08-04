using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShubT.Services.CouponsAPI.Data;
using ShubT.Services.CouponsAPI.DTOs;
using ShubT.Services.CouponsAPI.Models;

namespace ShubT.Services.CouponsAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private ResponseDTO _responseDTO;

        public CouponAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _responseDTO = new ResponseDTO();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                var coupons = _context.Coupons.ToList();
                _responseDTO.Result = coupons;
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return Ok(_responseDTO);
        }


        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Id == id);
                var couponDTO = _mapper.Map<CouponDTO>(coupon);
                _responseDTO.Result = couponDTO;
                if (coupon == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.DisplayMessage = "Coupon not found";
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }

            return Ok(_responseDTO);
        }

        [HttpGet]
        [Route("[action]/{code}")]
        public IActionResult GetCouponByCode(string code)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                var couponDTO = _mapper.Map<CouponDTO>(coupon);
                _responseDTO.Result = couponDTO;
                if (coupon == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.DisplayMessage = "Coupon not found";
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return Ok(_responseDTO);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Create([FromBody] CouponDTO couponDTO)
        {
            try
            {
                if (couponDTO == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.DisplayMessage = "Coupon data is null";
                    return BadRequest(_responseDTO);
                }
                if (!ModelState.IsValid)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.DisplayMessage = "Coupon data is not valid";
                    return BadRequest(_responseDTO);
                }

                var coupon = _mapper.Map<Coupon>(couponDTO);
                _context.Coupons.Add(coupon);
                _context.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDTO.DiscountAmount * 100),
                    Name = couponDTO.CouponCode,
                    Currency = "inr",
                    Id = couponDTO.CouponCode,
                };
                var service = new Stripe.CouponService();
                service.Create(options);

                _responseDTO.Result = couponDTO;
                return CreatedAtAction(nameof(Get), new { id = coupon.Id }, _responseDTO);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return Ok(_responseDTO);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Update([FromBody] CouponDTO couponDTO)
        {
            try
            {
                var existingCoupon = _context.Coupons.FirstOrDefault(c => c.Id == couponDTO.Id);
                if (existingCoupon == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.DisplayMessage = "Oops! No such coupon exists.";
                    return NotFound(_responseDTO);
                }

                //_context.Coupons.Update(coupon); -- not working because I added the above check
                _context.Entry(existingCoupon).CurrentValues.SetValues(couponDTO);
                _context.SaveChanges();
                _responseDTO.Result = couponDTO;
                return CreatedAtAction(nameof(Get), new { id = couponDTO.Id }, _responseDTO);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return Ok(_responseDTO);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(int id)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Id == id);
                if (coupon == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.DisplayMessage = "Oops! No such coupon exists.";
                    return NotFound(_responseDTO);
                }
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();

                var service = new Stripe.CouponService();
                service.Delete(coupon.CouponCode);

                _responseDTO.Result = true;
                return Ok(_responseDTO);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return Ok(_responseDTO);
        }
    }
}
