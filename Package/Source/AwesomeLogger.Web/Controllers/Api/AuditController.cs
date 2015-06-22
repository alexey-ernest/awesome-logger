using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using AwesomeLogger.Web.Models;
using AwesomeLogger.Web.Services;

namespace AwesomeLogger.Web.Controllers.Api
{
    [Authorize]
    public class AuditController : ApiController
    {
        private readonly IAuditService _auditService;
        private readonly ISubscriptionService _subscriptionService;

        public AuditController(IAuditService auditService, ISubscriptionService subscriptionService)
        {
            _auditService = auditService;
            _subscriptionService = subscriptionService;
        }

        [Route("api/audit/{id:int}")]
        [EnableQuery]
        public async Task<IEnumerable<PatternMatchModel>> Get(int id, int skip, int take)
        {
            var sub = await _subscriptionService.GetAsync(id);
            var matches =
                await _auditService.SearchAsync(sub.MachineName, sub.LogPath, sub.Pattern, sub.Email, skip, take);
            return matches;
        }
    }
}