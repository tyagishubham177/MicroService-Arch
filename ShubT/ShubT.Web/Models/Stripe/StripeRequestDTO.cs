using ShubT.Web.Models.Orders;

namespace ShubT.Web.Models.Stripe
{
    public class StripeRequestDTO
    {
        public string? StripeSessionUrl { get; set; }
        public string? StripeSessionId { get; set; }
        public string ApprovedUrl { get; set; }
        public string CancelUrl { get; set; }
        public OrderHeaderDTO OrderHeaderDTO { get; set; }
    }
}
