using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Service;
using System.Threading.Tasks;
using static OnlineSalesTool.ApiParameter.ListingParams;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute]
    [Authorize]
    public class PosController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPosService _repo;

        public PosController(IPosService repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery]int count = 10,
            [FromQuery]int page = 1,
            [FromQuery]string type = "",
            [FromQuery]string contain = "",
            [FromQuery]string order = "",
            [FromQuery]bool asc = true)
        {
            var paramBuilder = new ParamBuilder()
                    .SetPage(page)
                    .SetItemPerPage(count)
                    .SetType(type).SetContain(contain)
                    .SetOrderBy(order)
                    .SetAsc(asc);
            return Ok(await _repo.Get(paramBuilder.Build()));
        }
    }
}
