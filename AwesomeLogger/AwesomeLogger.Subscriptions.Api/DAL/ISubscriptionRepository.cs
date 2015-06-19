using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AwesomeLogger.Subscriptions.Api.DAL
{
    public interface ISubscriptionRepository : IDisposable
    {
        Task<Subscription> GetAsync(int id);
        Task<IEnumerable<Subscription>> GetByMachineAsync(string machine);
        Task<Subscription> AddAsync(Subscription sub);
        Task<Subscription> UpdateAsync(Subscription sub);
    }
}