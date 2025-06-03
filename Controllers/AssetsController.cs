using Microsoft.AspNetCore.Mvc;
using UnityAssetStore.Data;

namespace UnityAssetStore.Controllers
{
    public class AssetsController : Controller
    {
        private readonly AppDbContext _context;

        public AssetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var assets = _context.Assets.ToList();
            return View(assets);
        }

        // GET: /<controller>/Details/5
        public IActionResult Details(int id)
        {
            var asset = _context.Assets.FirstOrDefault(a => a.Id == id);

            if (asset == null)
                return NotFound();

            return View(asset);
        }
    }
}
