using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Services;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using static OnlineSalesCore.Helper.Params;

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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateContract([FromBody]UpdateContractDTO dto)
        {
            if(!ModelState.IsValid) return BadRequest();
            try
            {
                await _service.UpdateContract(dto);
                return Ok();
            }
            catch (BussinessException ex)
            {
                _logger.LogDebug(ex.Message);
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Assign([FromBody]CaseAssignDTO dto)
        {
            if(!ModelState.IsValid) return BadRequest();
            try
            {
                await _service.Assign(dto);
                return Ok();
            }
            catch (BussinessException ex)
            {
                _logger.LogDebug(ex.Message);
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Confirm([FromBody]CustomerConfirmDTO dto)
        {
            if(!ModelState.IsValid) return BadRequest();
            try
            {
                await _service.Confirm(dto);
                return Ok();
            }
            catch (BussinessException ex)
            {
                _logger.LogDebug(ex.Message);
                return BadRequest(ex.Message);
            }

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
