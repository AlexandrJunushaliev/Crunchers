using System;
using System.Linq;
using System.Threading.Tasks;
using Arshinov.WebApp.Models;
using Arshinov.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Arshinov.WebApp.Controllers
{
    public class CharacteristicsController : Controller
    {
        private CharacteristicsService _characteristicsService;
        private CategoriesService _categoriesService;

        public CharacteristicsController(CharacteristicsService characteristicsService,
            CategoriesService categoriesService)
        {
            _characteristicsService = characteristicsService;
            _categoriesService = categoriesService;
        }


        // GET
        public async Task<IActionResult> Index(int categoryId)
        {
            var category = await _categoriesService.Find(categoryId);
            ViewData["categoryName"] = category.Name;
            ViewData["categoryId"] = categoryId;
            return View(category.Characteristics);
        }

        [HttpGet]
        public async Task<IActionResult> Add(int categoryId)
        {
            var category = await _categoriesService.Find(categoryId);
            ViewData["categoryName"] = category.Name;
            ViewData["categoryId"] = categoryId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(int categoryId, CharacteristicViewModel model)
        {
            await _characteristicsService.Add(new Characteristic()
            {
                Name = model.Name
            });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int categoryId)
        {
            return View(await _characteristicsService.Find(categoryId));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int categoryId, string action, CharacteristicViewModel model)
        {
            if (action == "Update")
            {
                Characteristic characteristic = await _characteristicsService.Find(categoryId);
                characteristic.Name = model.Name;
                await _characteristicsService.SaveChangesAsync(characteristic);
                return RedirectToAction("Index");
            }

            if (action == "Delete")
            {
                await _characteristicsService.RemoveById(categoryId);
                return RedirectToAction("Index");
            }

            throw new InvalidOperationException("incorrect action");
        }
    }
}