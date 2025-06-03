using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UnityAssetStore.Data;
using UnityAssetStore.Models;

namespace UnityAssetStore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        // Получить все категории
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Получить категорию по ID
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new ArgumentException("Категория не найдена", nameof(id));

            return category;
        }

        // Добавить новую категорию
        public async Task AddCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        // Обновить существующую категорию
        public async Task UpdateCategoryAsync(int id, Category updatedCategory)
        {
            var existingCategory = await _context.Categories.FindAsync(id);

            if (existingCategory == null)
                throw new ArgumentException("Категория не найдена", nameof(id));

            existingCategory.Name = updatedCategory.Name;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();
        }

        // Удалить категорию по ID
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                throw new ArgumentException("Категория не найдена", nameof(id));

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}