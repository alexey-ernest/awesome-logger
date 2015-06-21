using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeLogger.Web.Models;

namespace AwesomeLogger.Web.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionModel>> GetAllAsync();

        Task<SubscriptionModel> GetAsync(int id);

        Task<SubscriptionModel> AddAsync(SubscriptionModel sub);

        Task<SubscriptionModel> UpdateAsync(SubscriptionModel sub);

        Task DeleteAsync(int id);
    }
}