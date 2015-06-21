using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
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

        [EnableQuery]
        [AuthorizeInternal]
        [Route("")]
        public IQueryable<Subscription> Get()
        {
            IQueryable<Subscription> subs = _db.AsQueryable();
            return subs;
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
            var sub = new Subscription
            {
                Id = id,
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
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            try
            {
                var sub = await _db.GetAsync(id);
                await _db.DeleteAsync(sub);
            }
            catch (NotFoundException)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [AuthorizeExternal]
        [Route("machine/{name}")]
        public async Task<IEnumerable<Subscription>> GetByMachine(string name)
        {
            var subs = await _db.GetByMachineAsync(name);
            return subs;
        }
    }
}