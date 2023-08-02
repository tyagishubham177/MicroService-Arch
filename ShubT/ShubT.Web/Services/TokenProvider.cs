using Newtonsoft.Json.Linq;
using ShubT.Web.Services.Interfaces;
using ShubT.Web.Utils;

namespace ShubT.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(MiscUtils.TokenCookie);
        }

        public string GetToken()
        {
            string token = null;
            bool hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(MiscUtils.TokenCookie, out token) ?? false;
            return hasToken ? token : null;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(MiscUtils.TokenCookie, token);
        }
    }
}
