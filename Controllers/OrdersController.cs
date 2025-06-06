using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnityAssetStore.Models;
using System.Security.Claims;
using UnityAssetStore.Services;

namespace UnityAssetStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /Orders/Index
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        // GET: /Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId)
                return Forbid();

            return View(order);
        }

        // POST: /Orders/Checkout — для оформления из корзины
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            try
            {
                int orderId = await _orderService.CreateOrderFromCartAsync(userId);
                return RedirectToAction("OrderConfirmed", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"Ошибка при оформлении заказа: {ex.Message}");
                return RedirectToAction("Index", "Cart");
            }
        }

        // POST: /Orders/CreateDirect?assetId=... — прямое оформление
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirect(int assetId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            try
            {
                int orderId = await _orderService.CreateSingleItemOrderAsync(userId, assetId);
                return RedirectToAction("OrderConfirmed", new { id = orderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при прямом оформлении заказа: {ex.Message}");
                return RedirectToAction("Details", "Assets", new { id = assetId });
            }
        }

        // GET: /Orders/OrderConfirmed
        public IActionResult OrderConfirmed(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}