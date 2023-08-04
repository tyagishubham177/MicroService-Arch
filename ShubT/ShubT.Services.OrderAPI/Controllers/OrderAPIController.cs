using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShubT.MessageBus;
using ShubT.Services.OrderAPI.Data;
using ShubT.Services.OrderAPI.DTOs;
using ShubT.Services.OrderAPI.Models;
using ShubT.Services.OrderAPI.Service.Interfaces;
using ShubT.Web.Utils;
using Stripe;
using Stripe.Checkout;

namespace ShubT.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO _responseDTO;
        private IMapper _mapper;
        private readonly AppDbContext _context;
        private IProductService _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderAPIController(AppDbContext context, IProductService productService, IMapper mapper,
            IConfiguration configuration, IMessageBus messageBus)
        {
            _context = context;
            this._responseDTO = new ResponseDTO();
            _messageBus = messageBus;
            _productService = productService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDTO? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(MiscUtils.RoleAdmin))
                {
                    objList = _context.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                else
                {
                    objList = _context.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderHeaderId).ToList();
                }
                _responseDTO.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(objList);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return _responseDTO;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDTO? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _context.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                _responseDTO.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return _responseDTO;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDto)
        {
            try
            {
                OrderHeaderDTO orderHeaderDto = _mapper.Map<OrderHeaderDTO>(cartDto.CartHeaderDTO);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = MiscUtils.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDto.CartDetailsDTO);
                orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);
                OrderHeader orderCreated = _context.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _context.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _responseDTO.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.DisplayMessage = ex.Message;
            }
            return _responseDTO;
        }


        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDto)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",

                };

                var DiscountsObj = new List<SessionDiscountOptions>();
                var discountOption = new SessionDiscountOptions();
                discountOption.Coupon = stripeRequestDto.OrderHeaderDTO.CouponCode;
                DiscountsObj.Add(discountOption);


                foreach (var item in stripeRequestDto.OrderHeaderDTO.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions();
                    sessionLineItem.PriceData = new SessionLineItemPriceDataOptions();
                    sessionLineItem.PriceData.ProductData = new SessionLineItemPriceDataProductDataOptions();

                    sessionLineItem.PriceData.ProductData.Name = item.ProductDTO.Name;
                    sessionLineItem.PriceData.UnitAmount = (long)(Math.Round(item.Price, 2) * 100);
                    sessionLineItem.PriceData.Currency = "inr";
                    sessionLineItem.Quantity = item.Count;
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.OrderHeaderDTO.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _context.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeaderDTO.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _context.SaveChanges();
                _responseDTO.Result = stripeRequestDto;

            }
            catch (Exception ex)
            {
                _responseDTO.DisplayMessage = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }


        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {

                OrderHeader orderHeader = _context.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    //payment was successful
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = MiscUtils.Status_Approved;
                    _context.SaveChanges();
                    RewardsDTO rewardsDTO = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardsDTO, topicName);

                    _responseDTO.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }

            }
            catch (Exception ex)
            {
                _responseDTO.DisplayMessage = ex.Message;
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _context.OrderHeaders.First(u => u.OrderHeaderId == orderId);
                if (orderHeader != null)
                {
                    if (newStatus == MiscUtils.Status_Cancelled)
                    {
                        //we will give refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund = service.Create(options);
                    }
                    orderHeader.Status = newStatus;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }
    }
}