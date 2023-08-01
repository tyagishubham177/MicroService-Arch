using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShubT.Web.Models;
using ShubT.Web.Services.Interfaces;

namespace ShubT.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO> coupons = new();
            var resp = await _couponService.GetAllCouponsAsync();
            if (resp != null && resp.IsSuccess)
            {
                coupons = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(resp.Result));
            }
            else
            {
                TempData["Error"] = resp.DisplayMessage;
            }

            return View(coupons);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO coupon)
        {
            if (ModelState.IsValid)
            {
                var resp = await _couponService.CreateCouponAsync(coupon);
                if (resp != null && resp.IsSuccess)
                {
                    TempData["Success"] = "Coupon created";
                    return RedirectToAction(nameof(CouponIndex));

                }
                else
                {
                    TempData["Error"] = resp.DisplayMessage;
                }
            }
            return View(coupon);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            var resp = await _couponService.GetCouponByIdAsync(couponId);
            if (resp != null && resp.IsSuccess)
            {
                CouponDTO model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(resp.Result));
                return View(model);
            }
            else
            {
                TempData["Error"] = resp.DisplayMessage;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO couponDTO)
        {
            var resp = await _couponService.DeleteCouponAsync(couponDTO.Id);
            if (resp != null && resp.IsSuccess)
            {
                TempData["Success"] = "Coupon Deleted";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["Error"] = resp.DisplayMessage;
            }
            return View(couponDTO);
        }
    }
}
