using Microsoft.EntityFrameworkCore;
using UnityAssetStore.Data;
using UnityAssetStore.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace UnityAssetStore.Services
{
    public class AssetService : IAssetService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AssetService(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // Получить все товары (с категориями)
        public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
        {
            return await _context.Assets
                .Include(a => a.Category)
                .ToListAsync();
        }

        // Получить товар по ID (с категорией)
        public async Task<Asset> GetAssetByIdAsync(int id)
        {
            var asset = await _context.Assets
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null)
                throw new ArgumentException("Товар не найден", nameof(id));

            return asset;
        }

        // Добавить новый товар
        public async Task AddAssetAsync(Asset asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));

            await _context.Assets.AddAsync(asset);
            await _context.SaveChangesAsync();
        }

        // Обновить существующий товар
        public async Task UpdateAssetAsync(int id, Asset updatedAsset)
        {
            var existingAsset = await _context.Assets.FindAsync(id);
            if (existingAsset == null) throw new ArgumentException("Товар не найден", nameof(id));

            // Удалить старое изображение, если выбрано новое
            if (!string.IsNullOrEmpty(existingAsset.PreviewImageUrl))
            {
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", Path.GetFileName(existingAsset.PreviewImageUrl));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            existingAsset.Name = updatedAsset.Name;
            existingAsset.Description = updatedAsset.Description;
            existingAsset.Price = updatedAsset.Price;
            existingAsset.PreviewImageUrl = updatedAsset.PreviewImageUrl;
            existingAsset.CategoryId = updatedAsset.CategoryId;

            _context.Assets.Update(existingAsset);
            await _context.SaveChangesAsync();
        }

        // Удалить товар по ID
        public async Task DeleteAssetAsync(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
                throw new ArgumentException("Товар не найден", nameof(id));

            // Проверяем, является ли изображение дефолтным
            if (asset.PreviewImageUrl != "/img/default.jpg")
            {
                // Удаление только если это не дефолтное изображение
                try
                {
                    // Здесь можно добавить логику для удаления файла из wwwroot/img
                    // Например:
                    // string imagePath = Path.Combine(_hostEnvironment.WebRootPath, "img", Path.GetFileName(asset.PreviewImageUrl));
                    // if (System.IO.File.Exists(imagePath))
                    // {
                    //     System.IO.File.Delete(imagePath);
                    // }

                    _context.Assets.Remove(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    throw new InvalidOperationException(
                        "Не удалось удалить товар. Возможно, он используется в других записях.", ex);
                }
            }
            else
            {
                // Для дефолтного изображения просто удаляем запись в базе данных
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Asset>> SearchAssetsAsync(string query)
        {
            var lowerQuery = query.ToLower();

            return await _context.Assets
                .Include(a => a.Category)
                .Where(a => a.Name.ToLower().Contains(lowerQuery) ||
                            (a.Category != null && a.Category.Name.ToLower().Contains(lowerQuery)))
                .ToListAsync();
        }
    }
}