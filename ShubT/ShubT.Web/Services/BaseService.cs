using Newtonsoft.Json;
using ShubT.Web.Models;
using ShubT.Web.Services.Interfaces;
using System.Net;
using System.Text;

namespace ShubT.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory clientFactory, ITokenProvider tokenProvider)
        {
            _clientFactory = clientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                HttpClient httpClient = _clientFactory.CreateClient("ShubTAPI");
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                message.RequestUri = new Uri(requestDTO.Url);

                message.Method = requestDTO.ApiType switch
                {
                    ApiType.GET => HttpMethod.Get,
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };

                if (requestDTO.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? responseMessage = null;

                responseMessage = await httpClient.SendAsync(message);

                switch (responseMessage.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            DisplayMessage = "Resource not found"
                        };
                    case HttpStatusCode.Unauthorized:
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            DisplayMessage = "Unauthorized"
                        };
                    case HttpStatusCode.BadRequest:
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            DisplayMessage = "Bad Request"
                        };
                    case HttpStatusCode.Forbidden:
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            DisplayMessage = "Forbidden"
                        };
                    case HttpStatusCode.InternalServerError:
                        return new ResponseDTO
                        {
                            IsSuccess = false,
                            DisplayMessage = "Internal Server Error"
                        };
                    default:
                        {
                            var apiContent = await responseMessage.Content.ReadAsStringAsync();
                            var responseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                            return responseDTO;
                        }
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    DisplayMessage = ex.Message
                };
            }
        }
    }
}
