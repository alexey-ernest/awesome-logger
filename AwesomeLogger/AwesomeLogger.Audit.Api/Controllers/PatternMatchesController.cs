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
    [ValidationHttp]
    public class PatternMatchesController : ApiController
    {
        private readonly IPatternMatchRepository _db;

        public PatternMatchesController(IPatternMatchRepository db)
        {
            _db = db;
        }

        [Route("")]
        public async Task<HttpResponseMessage> Post(PatternMatch model)
        {
            model = await _db.AddAsync(model);

            var response = Request.CreateResponse(HttpStatusCode.Created, model);
            return response;
        }

        [Route("search?m={machine}&s={searchPath}&p={pattern}&e={email}")]
        public async Task<IEnumerable<PatternMatch>> GetByMachine(string machine, string searchPath, string pattern, string email)
        {
            var subs = await _db.GetRelatedAsync(machine, searchPath, pattern, email);
            return subs;
        }
    }
}