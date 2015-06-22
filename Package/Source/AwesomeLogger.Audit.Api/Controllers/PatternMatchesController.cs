using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using AwesomeLogger.Audit.Api.DAL;
using AwesomeLogger.Audit.Api.Infrastructure.Filters;

namespace AwesomeLogger.Audit.Api.Controllers
{
    [AuthorizeInternal]
    [Authorize]
    [Route("")]
    public class PatternMatchesController : ApiController
    {
        private readonly IPatternMatchRepository _db;

        public PatternMatchesController(IPatternMatchRepository db)
        {
            _db = db;
        }

        [ValidationHttp]
        public async Task<HttpResponseMessage> Post(PatternMatch model)
        {
            model = await _db.AddAsync(model);

            var response = Request.CreateResponse(HttpStatusCode.Created, model);
            return response;
        }

        [EnableQuery]
        public IQueryable<PatternMatch> Get(string m, string s, string p, string e)
        {
            return _db.Query(m, s, p, e);
        }
    }
}