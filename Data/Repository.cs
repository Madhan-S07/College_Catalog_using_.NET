using Microsoft.EntityFrameworkCore;
using CollegeManagementApi.Interface;

namespace CollegeManagementApi.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;

        public Repository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
        }

        public void Remove(T entity)
        {
            _db.Set<T>().Remove(entity);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}