using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AwesomeLogger.Subscriptions.Api.DAL;
using AwesomeLogger.Subscriptions.Api.Exceptions;
using AwesomeLogger.Subscriptions.Api.Infrastructure.Filters;
using AwesomeLogger.Subscriptions.Api.Models;

namespace AwesomeLogger.Subscriptions.Api.Controllers
{
    [Authorize]
    [ValidationHttp]
    public class SubscriptionsController : ApiController
    {
        private readonly ISubscriptionRepository _db;

        public SubscriptionsController(ISubscriptionRepository db)
        {
            _db = db;
        }

        [AuthorizeInternal]
        [Route("")]
        public Task<Subscription> Get()
        {
            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden));
        }

        [AuthorizeInternal]
        [Route("{id:int}")]
        public async Task<Subscription> Get(int id)
        {
            var sub = await _db.GetAsync(id);
            if (sub == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return sub;
        }

        [AuthorizeInternal]
        [Route("")]
        public async Task<HttpResponseMessage> Post(SubscriptionCreateUpdateModel model)
        {
            if (model == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            var sub = new Subscription
            {
                MachineName = model.MachineName,
                LogPath = model.LogPath,
                Pattern = model.Pattern,
                Email = model.Email
            };
            sub = await _db.AddAsync(sub);

            var response = Request.CreateResponse(HttpStatusCode.Created, sub);
            return response;
        }

        [AuthorizeInternal]
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Put(int id, SubscriptionCreateUpdateModel model)
        {
            if (model == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            }

            var sub = new Subscription
            {
                MachineName = model.MachineName,
                LogPath = model.LogPath,
                Pattern = model.Pattern,
                Email = model.Email
            };

            try
            {
                sub = await _db.UpdateAsync(sub);
            }
            catch (NotFoundException)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, sub);
            return response;
        }

        [AuthorizeInternal]
        [AuthorizeExternal]
        [Route("machine/{machine}")]
        public async Task<IEnumerable<Subscription>> GetByMachine(string machine)
        {
            if (string.IsNullOrEmpty(machine))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden));
            }

            var subs = await _db.GetByMachineAsync(machine);
            return subs;
        }
    }
}