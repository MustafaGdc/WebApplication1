using WebApplication1.Models;

namespace WebApplication1.Repositories.Interfaces
{
    public interface IFileRepository : IGenericRepository<StoredFile>
    {
        Task<IEnumerable<StoredFile>> GetAllWithCategoryAsync();
        Task<StoredFile?> GetByIdWithCategoryAsync(int id);
    }
}