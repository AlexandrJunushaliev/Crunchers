using System;
using System.Threading.Tasks;
using Arshinov.WebApp.Models;
using Arshinov.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Arshinov.WebApp.Controllers
{
    public class CharacteristicEnumsController : Controller
    {
        private CharacteristicEnumsService _characteristicEnumsService;

        public CharacteristicEnumsController(CharacteristicEnumsService characteristicEnumsService)
        {
            _characteristicEnumsService = characteristicEnumsService;
        }


        // GET
        public IActionResult Index()
        {
            return View(_characteristicEnumsService.GetAll());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CharacteristicEnumViewModel model)
        {
            await _characteristicEnumsService.Add(new CharacteristicEnum()
            {
                //Name = model.Name
            });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int categoryId)
        {
            return View(await _characteristicEnumsService.Find(categoryId));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int categoryId, string action, CharacteristicEnumViewModel model)
        {
            if (action == "Update")
            {
                CharacteristicEnum characteristicEnum = await _characteristicEnumsService.Find(categoryId);
                //characteristicEnum.Name = model.Name;
                await _characteristicEnumsService.SaveChangesAsync(characteristicEnum);
                return RedirectToAction("Index");
            }
            if (action == "Delete")
            {
                await _characteristicEnumsService.RemoveById(categoryId);
                return RedirectToAction("Index");
            }
            throw new InvalidOperationException("incorrect action");
        }
    }
}