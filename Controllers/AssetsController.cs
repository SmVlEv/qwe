using Microsoft.AspNetCore.Mvc;
using UnityAssetStore.Data;
using UnityAssetStore.Models;
using UnityAssetStore.Services;
using System.Threading.Tasks;

namespace UnityAssetStore.Controllers
{
    public class AssetsController : Controller
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        // GET: /Assets/Index
        public async Task<IActionResult> Index(string searchQuery)
        {
            IEnumerable<Asset> assets;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Поиск по имени товара или категории
                assets = await _assetService.SearchAssetsAsync(searchQuery.Trim());
            }
            else
            {
                assets = await _assetService.GetAllAssetsAsync();
            }

            return View(assets);
        }

        // GET: /Assets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }
    }
}