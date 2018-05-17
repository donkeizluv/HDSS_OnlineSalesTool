using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OnlineSalesTool.ApiParameter.ListingParams;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute]
    [Authorize]
    public class PosController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPosRepository _repo;

        public PosController(IPosRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery]int page = 1, [FromQuery]string type = "", [FromQuery]string contain = "", [FromQuery]string order = "", [FromQuery]bool asc = true)
        {
            var paramBuilder = new ParamBuilder()
                    .SetPage(page)
                    .SetType(type).SetContain(contain)
                    .SetOrderBy(order)
                    .SetAsc(asc);
            return Ok(await _repo.Get(paramBuilder.Build()));
        }

    }
}
