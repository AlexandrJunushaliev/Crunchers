using System;
using System.Linq;
using System.Threading.Tasks;
using Arshinov.WebApp.Models;
using Arshinov.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Arshinov.WebApp.Controllers
{
    public class CategoriesController : Controller
    {
        private CategoriesService _categoriesService;

        public CategoriesController(CategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }


        // GET
        public IActionResult Index()
        {
            return View(_categoriesService.GetAll());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryViewModel model)
        {
            await _categoriesService.Add(new Category()
            {
                Name = model.Name
            });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int categoryId)
        {
            return View(await _categoriesService.Find(categoryId));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int categoryId, string action, CategoryViewModel model)
        {
            if (action == "Update")
            {
                Category category = await _categoriesService.Find(categoryId);
                category.Name = model.Name;
                await _categoriesService.SaveChangesAsync(category);
                return RedirectToAction("Index");
            }
            if (action == "Delete")
            {
                await _categoriesService.RemoveById(categoryId);
                return RedirectToAction("Index");
            }

            if (action == "Edit Characteristics")
            {
                return RedirectToAction("Index", "Characteristics", new {categoryId});
            }
            throw new InvalidOperationException("incorrect action");
        }
    }
}