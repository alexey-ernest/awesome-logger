using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeLogger.Subscriptions.Api.DAL
{
    public interface ISubscriptionRepository : IDisposable
    {
        IQueryable<Subscription> AsQueryable();

        Task<Subscription> GetAsync(int id);

        Task<IEnumerable<Subscription>> GetByMachineAsync(string machine);

        Task<Subscription> AddAsync(Subscription sub);

        Task<Subscription> UpdateAsync(Subscription sub);

        Task DeleteAsync(Subscription sub);
    }
}