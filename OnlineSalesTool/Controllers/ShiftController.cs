using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Services;
using OnlineSalesCore.ViewModels;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Helper;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(ShiftController))]
    public class ShiftController : Controller
    {
        private readonly ILogger<ShiftController> _logger;
        private readonly IScheduleService _service;

        public ShiftController(IScheduleService service, ILogger<ShiftController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.Get());
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDetails(int? id)
        {
            if (id == null) return BadRequest();
            return Ok(await _service.GetDetail(id ?? -1));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]ScheduleContainer schedule)
        {
            if (!ModelState.IsValid || schedule == null) return BadRequest("Invalid post data");
            try
            {
                var id = await _service.Create(schedule);
                return Ok(id);
            }
            catch (BussinessException ex) //Fail bussiness check
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest("System error.");
            }
            catch (SqlException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest("System error.");
            }

        }
    }
}
