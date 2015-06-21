using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AwesomeLogger.Subscriptions.Api.Exceptions;

namespace AwesomeLogger.Subscriptions.Api.DAL
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly SubscriptionDbContext _db;

        public SubscriptionRepository(SubscriptionDbContext db)
        {
            _db = db;
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public IQueryable<Subscription> AsQueryable()
        {
            return _db.Subscriptions;
        }

        public async Task<Subscription> GetAsync(int id)
        {
            return await _db.Subscriptions.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetByMachineAsync(string machine)
        {
            return await _db.Subscriptions.Where(s => s.MachineName == machine).ToListAsync();
        }

        public async Task<Subscription> AddAsync(Subscription sub)
        {
            sub.Created = DateTime.UtcNow;
            sub = _db.Subscriptions.Add(sub);
            await _db.SaveChangesAsync();
            return sub;
        }

        public async Task<Subscription> UpdateAsync(Subscription sub)
        {
            var original = await GetAsync(sub.Id);
            if (original == null)
            {
                throw new NotFoundException("Subscription does not exist.");
            }

            sub.Created = original.Created;
            _db.Entry(original).CurrentValues.SetValues(sub);
            await _db.SaveChangesAsync();
            return original;
        }

        public async Task DeleteAsync(Subscription sub)
        {
            sub = await GetAsync(sub.Id);
            if (sub == null)
            {
                throw new NotFoundException("Subscription does not exist.");
            }

            _db.Subscriptions.Remove(sub);
            await _db.SaveChangesAsync();
        }
    }
}