namespace CollegeManagementApi.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Remove(T entity);
        Task SaveAsync();
    }
}