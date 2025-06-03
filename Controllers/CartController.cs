using Microsoft.AspNetCore.Mvc;
using UnityAssetStore.Services;
using UnityAssetStore.Models;
using System.Security.Claims;

namespace UnityAssetStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /Cart/Index
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItems = _cartService.GetCartItems(userId);

                // Возвращаем список товаров как ShoppingCart
                var shoppingCart = new ShoppingCart();
                foreach (var item in cartItems)
                {
                    shoppingCart.Items.Add(item); // Item == OrderItem
                }

                return View(shoppingCart);
            }
            else
            {
                var cart = _cartService.GetCart(HttpContext.Session);
                return View(cart);
            }
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int assetId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _cartService.AddToCartAsync(userId, assetId).GetAwaiter().GetResult(); // Синхронная версия
            }
            else
            {
                _cartService.AddToCart(HttpContext.Session, assetId);
            }
            TempData["SuccessMessage"] = "Товар успешно добавлен в корзину!";
            return RedirectToAction("Index", "Assets");
        }

        // POST: /Cart/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int assetId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _cartService.RemoveFromCartAsync(userId, assetId).GetAwaiter().GetResult(); // Синхронная версия
            }
            else
            {
                _cartService.RemoveFromCart(HttpContext.Session, assetId);
            }

            return RedirectToAction("Index");
        }
    }
}