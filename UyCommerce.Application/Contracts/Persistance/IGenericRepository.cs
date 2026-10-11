using System;
using System.Collections.Generic;
using System.Text;

namespace UyCommerce.Application.Contracts.Persistance
{
    public interface IGenericRepository<T> where T : class
    {
        Task<int> CreateAsync(T entity);
        Task<T?> GetByIdNoTrackingAsync(int id);
        Task<T?> GetByIdAsync(int id);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<List<T>> GetAllAsync();
    }
}
