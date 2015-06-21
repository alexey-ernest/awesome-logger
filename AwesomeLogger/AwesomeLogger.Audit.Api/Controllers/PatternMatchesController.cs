using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AwesomeLogger.Audit.Api.DAL;
using AwesomeLogger.Audit.Api.Infrastructure.Filters;

namespace AwesomeLogger.Audit.Api.Controllers
{
    [AuthorizeInternal]
    [Authorize]
    public class PatternMatchesController : ApiController
    {
        private readonly IPatternMatchRepository _db;

        public PatternMatchesController(IPatternMatchRepository db)
        {
            _db = db;
        }

        [ValidationHttp]
        [Route("")]
        public async Task<HttpResponseMessage> Post(PatternMatch model)
        {
            model = await _db.AddAsync(model);

            var response = Request.CreateResponse(HttpStatusCode.Created, model);
            return response;
        }

        [Route("")]
        public async Task<IEnumerable<PatternMatch>> Get(string m, string s, string p, string e)
        {
            var matches = await _db.GetRelatedAsync(m, s, p, e);
            return matches;
        }
    }
}