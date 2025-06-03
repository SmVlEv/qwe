using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UnityAssetStore.Data;
using UnityAssetStore.Models;
using UnityAssetStore.Services;

namespace UnityAssetStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAssetService _assetService;

        public DashboardController(AppDbContext context, IAssetService assetService)
        {
            _context = context;
            _assetService = assetService;
        }

        // GET: /Admin/Dashboard/Index
        public IActionResult Index()
        {
            var assets = _context.Assets.Include(a => a.Category).ToList();
            return View(assets);
        }

        // GET: /Admin/Dashboard/Add
        [HttpGet]
        public IActionResult Add()
        {
            SetCategorySelectList();
            return View();
        }

        // POST: /Admin/Dashboard/Add
        [HttpPost]
        public IActionResult Add(Asset model)
        {
            if (!ModelState.IsValid)
            {
                SetCategorySelectList();
                return View(model);
            }

            _context.Assets.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Admin/Dashboard/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var asset = _context.Assets.Find(id);
            if (asset == null) return NotFound();

            SetCategorySelectList(asset.CategoryId);
            return View(asset);
        }

        // POST: /Admin/Dashboard/Edit
        [HttpPost]
        public IActionResult Edit(Asset model)
        {
            if (!ModelState.IsValid)
            {
                SetCategorySelectList(model.CategoryId);
                return View(model);
            }

            _context.Assets.Update(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Admin/Dashboard/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var asset = _context.Assets.Find(id);
            if (asset == null) return NotFound();

            return View(asset);
        }

        // POST: /Admin/Dashboard/DeleteConfirmed
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var asset = _context.Assets.Find(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Вспомогательный метод для заполнения выпадающего списка категорий
        private void SetCategorySelectList(int selectedCategoryId = 0)
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
        }
    }
}