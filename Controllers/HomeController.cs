using Microsoft.AspNetCore.Mvc;
using UnityAssetStore.Models;
using UnityAssetStore.Services;

namespace UnityAssetStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly ICategoryService _categoryService;

        public HomeController(IAssetService assetService, ICategoryService categoryService)
        {
            _assetService = assetService;
            _categoryService = categoryService;
        }

        // GET: /Home/Index?categoryId=...
        public async Task<IActionResult> Index(int? categoryId)
        {
            IEnumerable<Asset> assets;

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                // ������ �� ���������
                assets = await _assetService.GetAssetsByCategoryIdAsync(categoryId.Value);
            }
            else
            {
                // ��� ������
                assets = await _assetService.GetAllAssetsAsync();
            }

            // �������� ������ ��������� � ViewBag ��� ����������� ������
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();

            return View(assets);
        }
    }
}