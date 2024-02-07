using System.Linq.Expressions;
using Villa_API.Models;

namespace Villa_API.Repository.IRepository
{
    public interface IVillaRepository
    {
        Task<List<Villa>> GetAll(Expression<Func<Villa>> filter = null);
        Task<Villa> Get(Expression<Func<Villa>> filter, bool tracked = true);
        Task Create(Villa entity);
        Task Update(Villa entity);
        Task Remove(Villa entity);
        Task Save();

    }
}
