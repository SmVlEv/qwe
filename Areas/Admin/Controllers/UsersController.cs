using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnityAssetStore.Models.Identity;

namespace UnityAssetStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Только администратор может использовать
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Admin/Users/Index
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            // Получаем роли для всех пользователей
            var roles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Admin"))
                {
                    roles[user.Id] = "Admin";
                }
                else
                {
                    roles[user.Id] = "User";
                }
            }

            ViewBag.Roles = roles; // Передаём роли в представление

            return View(users);
        }

        // POST: /Admin/Users/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Запрет удаления самого себя
            if (User.Identity?.Name == user.UserName)
            {
                ModelState.AddModelError("", "Невозможно удалить свой собственный аккаунт");
                return View("Index", await _userManager.Users.ToListAsync());
            }

            // Проверка роли
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                ModelState.AddModelError("", "Невозможно удалить администратора");
                return View("Index", await _userManager.Users.ToListAsync());
            }

            // Удаление пользователя
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}