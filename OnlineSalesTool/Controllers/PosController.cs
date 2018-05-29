using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.DTO;
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
        private readonly IPosService _service;

        public PosController(IPosService service)
        {
            _service = service;
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
            return Ok(await _service.Get(paramBuilder.Build()));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] PosDTO user)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                return Ok(await _service.Create(user));
            }
            catch (BussinessException ex)
            {
                Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] PosDTO user)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                await _service.Update(user);
                return Ok();
            }
            catch (BussinessException ex)
            {
                Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
        }
    }
}
