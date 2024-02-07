using System.Linq.Expressions;
using Villa_API.Data;
using Villa_API.Models;
using Villa_API.Repository.IRepository;

namespace Villa_API.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _db;
        public async Task Create(Villa entity)
        {
            await _db.Villas.AddAsync(entity);
            await Save();
        }

        public Task<Villa> Get(Expression<Func<Villa>> filter, bool tracked = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<Villa>> GetAll(Expression<Func<Villa>> filter = null)
        {
            
        }

        public async Task Remove(Villa entity)
        {
            _db.Villas.Remove(entity);
            await Save();
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }

        public Task Update(Villa entity)
        {
            throw new NotImplementedException();
        }
    }
}
