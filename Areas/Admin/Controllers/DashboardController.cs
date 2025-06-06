using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UnityAssetStore.Models;
using UnityAssetStore.Services;
using UnityAssetStore.Data;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UnityAssetStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Только администратор может использовать
    public class DashboardController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DashboardController(
            IAssetService assetService,
            AppDbContext context,
            IWebHostEnvironment hostEnvironment)
        {
            _assetService = assetService;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: /Admin/Dashboard/Index
        public async Task<IActionResult> Index()
        {
            var assets = await _assetService.GetAllAssetsAsync();
            return View(assets);
        }

        // GET: /Admin/Dashboard/Add
        [HttpGet]
        public IActionResult Add()
        {
            SetCategorySelectList();
            SetImagesSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Asset model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                SetCategorySelectList();
                SetImagesSelectList();
                return View(model);
            }

            // Если загружено новое изображение
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var webRootPath = _hostEnvironment.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(webRootPath, "img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                model.PreviewImageUrl = $"/img/{fileName}";
            }
            else if (!string.IsNullOrEmpty(model.PreviewImageUrl) && !model.PreviewImageUrl.Contains("default.jpg"))
            {
                // Если выбрано изображение из списка
                model.PreviewImageUrl = model.PreviewImageUrl;
            }
            else
            {
                // Если ничего не выбрано — используем дефолтное изображение
                model.PreviewImageUrl = "/img/default.jpg";
            }

            try
            {
                await _assetService.AddAssetAsync(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при добавлении товара: {ex.Message}");
                SetCategorySelectList();
                SetImagesSelectList();
                return View(model);
            }
        }

        // GET: /Admin/Dashboard/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null) return NotFound();

            SetCategorySelectList(asset.CategoryId);
            SetImagesSelectList(); // Для выбора изображения при редактировании
            return View(asset);
        }

        // POST: /Admin/Dashboard/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Asset model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                SetCategorySelectList(model.CategoryId);
                SetImagesSelectList();
                return View(model);
            }

            // Если загружено новое изображение — удаляем старое и сохраняем новое
            if (ImageFile?.Length > 0)
            {
                // Удалить старое изображение, если оно не дефолтное
                if (!string.IsNullOrEmpty(model.PreviewImageUrl) &&
                    !model.PreviewImageUrl.Contains("default.jpg"))
                {
                    var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", Path.GetFileName(model.PreviewImageUrl));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Сохраняем новое изображение
                var webRootPath = _hostEnvironment.WebRootPath;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(webRootPath, "img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                model.PreviewImageUrl = $"/img/{fileName}";
            }

            await _assetService.UpdateAssetAsync(model.Id, model);
            return RedirectToAction("Index");
        }

        // GET: /Admin/Dashboard/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null) return NotFound();

            return View(asset);
        }

        // POST: /Admin/Dashboard/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null) return NotFound();

            // Не удалять изображение администратора или системные файлы
            if (!string.IsNullOrEmpty(asset.PreviewImageUrl) &&
                !asset.PreviewImageUrl.Contains("default.jpg"))
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", Path.GetFileName(asset.PreviewImageUrl));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _assetService.DeleteAssetAsync(id);
            return RedirectToAction("Index");
        }

        // Метод для заполнения выпадающего списка категорий
        private void SetCategorySelectList(int selectedCategoryId = 0)
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategoryId);
        }

        // Метод для заполнения выпадающего списка изображений
        private void SetImagesSelectList()
        {
            var images = Directory.GetFiles(Path.Combine(_hostEnvironment.WebRootPath, "img"))
                                  .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png"))
                                  .Select(file => new
                                  {
                                      Name = Path.GetFileName(file),
                                      Url = $"/img/{Path.GetFileName(file)}"
                                  })
                                  .ToList();

            ViewBag.Images = images;
        }
    }
}