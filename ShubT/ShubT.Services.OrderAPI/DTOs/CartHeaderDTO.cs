﻿namespace ShubT.Services.OrderAPI.DTOs
{
    public class CartHeaderDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        public double DiscountTotal { get; set; }
        public double CartTotal { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
