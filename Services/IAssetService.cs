using System.Collections.Generic;
using System.Threading.Tasks;
using UnityAssetStore.Models;

namespace UnityAssetStore.Services
{
    public interface IAssetService
    {
        Task<IEnumerable<Asset>> GetAllAssetsAsync();
        Task<Asset> GetAssetByIdAsync(int id);
        Task AddAssetAsync(Asset asset);
        Task UpdateAssetAsync(int id, Asset updatedAsset);
        Task DeleteAssetAsync(int id);
        Task<IEnumerable<Asset>> SearchAssetsAsync(string query);
    }
}