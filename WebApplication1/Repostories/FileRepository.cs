using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;

namespace WebApplication1.Repositories
{
    public class FileRepository : GenericRepository<StoredFile>, IFileRepository
    {
        public FileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StoredFile>> GetAllWithCategoryAsync()
        {
            return await _dbSet
                .Include(f => f.Category)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<StoredFile?> GetByIdWithCategoryAsync(int id)
        {
            return await _dbSet
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}