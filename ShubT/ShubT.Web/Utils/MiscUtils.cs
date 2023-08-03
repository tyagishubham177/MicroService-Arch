namespace ShubT.Web.Utils
{
    public class MiscUtils
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string ShoppingCartAPIBase { get; set; }

        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Customer";

        public const string TokenCookie = "JWTToken";
    }
}
