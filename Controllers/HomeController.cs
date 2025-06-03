using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnityAssetStore.Data;
using UnityAssetStore.Models;

namespace UnityAssetStore.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Главная";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }


}
