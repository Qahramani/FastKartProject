using FastKartProject.DataAccessLayer;
using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FastKartProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var categories = _dbContext.Categories.ToList();
            var product = _dbContext.Products.ToList();

            var model = new HomeViewModel()
            {
                Categories = categories,
                Products = product
            };
            return View(model);
        }

        public IActionResult Basket()
        {
            var basketInString = Request.Cookies["basket"];

            var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basketInString);

            var newBasketViewModels = new List<BasketViewModel>();
            foreach (var product in basketViewModels)
            {
                var existProduct = _dbContext.Products.Find(product.ProductId);

                if (existProduct is null) continue;

                newBasketViewModels.Add(new BasketViewModel()
                {
                    ProductId = existProduct.Id,
                    ImageUrl = existProduct.ImageUrl,
                    Price = existProduct.Price,
                    Name = existProduct.Name,
                    Count = product.Count
                });
            }

            return Json(newBasketViewModels);
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            var product = await _dbContext.Products.FindAsync(id);

            if (product is null)
                return BadRequest();

            var basketViewModel = new List<BasketViewModel>();

            if (string.IsNullOrEmpty(Request.Cookies["basket"]))
            {
                basketViewModel.Add(new BasketViewModel()
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Count = 1
                });
            }
            else
            {
                basketViewModel = JsonConvert.DeserializeObject<List<BasketViewModel>>(Request.Cookies["basket"]);

                var existProduct = basketViewModel.Find(x => x.ProductId == product.Id);

                if (existProduct == null)
                {
                    basketViewModel.Add(new BasketViewModel()
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        ImageUrl = product.ImageUrl,
                        Count = 1
                    });

                }
                else
                {
                    existProduct.Count++;
                }
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketViewModel));


            return RedirectToAction(nameof(Index));
        }


    }
}
