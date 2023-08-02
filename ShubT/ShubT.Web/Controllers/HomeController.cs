using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShubT.Web.Models;
using ShubT.Web.Models.Product;
using ShubT.Web.Services.Interfaces;
using System.Diagnostics;

namespace ShubT.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDTO> list = new();

            ResponseDTO response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.DisplayMessage;
            }

            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDTO product = new();

            ResponseDTO response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.DisplayMessage;
            }

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}