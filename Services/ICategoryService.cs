using System.Collections.Generic;
using System.Threading.Tasks;
using UnityAssetStore.Models;

namespace UnityAssetStore.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(int id, Category updatedCategory);
        Task DeleteCategoryAsync(int id);
    }
}