using ShubT.Services.EmailAPI.DTOs;
using System.Text;

namespace ShubT.Services.EmailAPI.Utils
{
    public class MiscUtils
    {
        public static string BuildHTMLCartString(CartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<html>");
            message.AppendLine("<head>");
            message.AppendLine("<style>");
            message.AppendLine("body {font-family: Arial; background-color: #f4f4f4; color: #333;}");
            message.AppendLine("table {width: 100%; border-collapse: collapse; background-color: #ffffff;}");
            message.AppendLine("th {background-color: #4CAF50; color: white; padding: 10px;}");
            message.AppendLine("td {border: 1px solid #ddd; padding: 10px;}");
            message.AppendLine("</style>");
            message.AppendLine("</head>");
            message.AppendLine("<body>");
            message.AppendLine("<h2>🛒 Cart Email Requested</h2>");
            message.AppendLine($"<p>User: {cartDTO.CartHeaderDTO.Name} ({cartDTO.CartHeaderDTO.Email})</p>");
            message.AppendLine("<table>");
            message.AppendLine("<tr>");
            message.AppendLine("<th>Product</th>");
            message.AppendLine("<th>Price (INR)</th>");
            message.AppendLine("<th>Count</th>");
            message.AppendLine("<th>Total (INR)</th>");
            message.AppendLine("</tr>");

            foreach (var item in cartDTO.CartDetailsDTO)
            {
                double totalProductPrice = item.Count * item.ProductDTO.Price; // Assuming ProductDTO has a Price property
                message.AppendLine("<tr>");
                message.AppendLine($"<td>{item.ProductDTO.Name}</td>");
                message.AppendLine($"<td>₹{item.ProductDTO.Price:F2}</td>");
                message.AppendLine($"<td>{item.Count} x 📦</td>");
                message.AppendLine($"<td>₹{totalProductPrice:F2}</td>");
                message.AppendLine("</tr>");
            }

            double cartTotalWithDiscount = cartDTO.CartHeaderDTO.CartTotal - cartDTO.CartHeaderDTO.DiscountTotal;

            message.AppendLine($"<tr><td colspan='3'>Coupon Code: {cartDTO.CartHeaderDTO.CouponCode ?? "N/A"}</td><td>₹{cartDTO.CartHeaderDTO.DiscountTotal:F2} 🎫</td></tr>");
            message.AppendLine($"<tr><td colspan='3'>Total:</td><td>₹{cartTotalWithDiscount:F2} 💰</td></tr>");
            message.AppendLine("</table>");
            message.AppendLine("</body>");
            message.AppendLine("</html>");

            return message.ToString();
        }
    }
}
