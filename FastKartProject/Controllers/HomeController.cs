using FastKartProject.DataAccessLayer;
using FastKartProject.DataAccessLayer.Entities;
using FastKartProject.Models;
using FastKartProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace FastKartProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private IBasketService _basketService;

        public HomeController(AppDbContext dbContext, IBasketService basketService)
        {
            _dbContext = dbContext;
            _basketService = basketService;
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

        public IActionResult GetBasket()
        {
            var newBasketViewModels = _basketService.GetBasket();

            return Json(newBasketViewModels);
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            try
            {
                await _basketService.AddToBasket(id);

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
