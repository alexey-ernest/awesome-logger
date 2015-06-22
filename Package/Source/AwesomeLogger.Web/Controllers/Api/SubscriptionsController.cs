using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using AwesomeLogger.Web.Infrastructure.Filters;
using AwesomeLogger.Web.Models;
using AwesomeLogger.Web.Services;

namespace AwesomeLogger.Web.Controllers.Api
{
    [Authorize]
    [ValidationHttp]
    [Route("api/subscriptions/{id:int?}")]
    public class SubscriptionsController : ApiController
    {
        private readonly ISubscriptionService _service;

        public SubscriptionsController(ISubscriptionService service)
        {
            _service = service;
        }

        [EnableQuery]
        public async Task<IEnumerable<SubscriptionModel>> Get()
        {
            IEnumerable<SubscriptionModel> subs = await _service.GetAllAsync();
            return subs;
        }

        public async Task<SubscriptionModel> Get(int id)
        {
            var sub = await _service.GetAsync(id);
            return sub;
        }

        public async Task<HttpResponseMessage> Post(SubscriptionModel model)
        {
            var sub = await _service.AddAsync(model);

            var response = Request.CreateResponse(HttpStatusCode.Created, sub);
            return response;
        }

        public async Task<HttpResponseMessage> Put(int id, SubscriptionModel model)
        {
            model.Id = id;
            var sub = await _service.UpdateAsync(model);

            var response = Request.CreateResponse(HttpStatusCode.OK, sub);
            return response;
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            await _service.DeleteAsync(id);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}