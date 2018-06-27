using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Service;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Helper;
using System.Threading.Tasks;
using static OnlineSalesCore.ApiParameter.ListingParams;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(PosController))]
    [Authorize]
    public class PosController : Controller
    {
        private readonly ILogger<PosController> _logger;
        private readonly IPosService _service;

        public PosController(IPosService service, ILogger<PosController> logger)
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] PosDTO pos)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                return Ok(await _service.Create(pos));
            }
            catch (BussinessException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] PosDTO pos)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                await _service.Update(pos);
                return Ok();
            }
            catch (BussinessException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Check([FromQuery] string q)
        {
            if(string.IsNullOrEmpty(q)) return BadRequest();
            return Ok(await _service.CheckCode(q.ToUpper()));
        }
    }
}
