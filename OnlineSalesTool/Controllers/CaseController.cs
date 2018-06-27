using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.Service;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using static OnlineSalesCore.ApiParameter.ListingParams;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(CaseController))]
    public class CaseController : Controller
    {
        private readonly ILogger<CaseController> _logger;
        private readonly ICaseService _service;
        public CaseController(ICaseService service, ILogger<CaseController> logger)
        {
            _service = service;
            _logger = logger;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(
            [FromQuery]int count = 10,
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
            return Ok(await _service.Get(paramBuilder.Build()));
        }
    }
}
