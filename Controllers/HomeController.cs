using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnityAssetStore.Data;
using UnityAssetStore.Models;
using UnityAssetStore.Services;

namespace UnityAssetStore.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IAssetService _assetService;

    public HomeController(AppDbContext context, IAssetService assetService)
    {
        _context = context;
        _assetService = assetService;
    }

    public async Task<IActionResult> Index()
    {
        var assets = await _assetService.GetAllAssetsAsync();
        return View(assets);
    }

    public IActionResult Privacy()
    {
        return View();
    }


}
