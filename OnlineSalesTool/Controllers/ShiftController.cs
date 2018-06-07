using OnlineSalesTool.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.Logic;
using OnlineSalesTool.Service;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Logging;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(ShiftController))]
    public class ShiftController : Controller
    {
        private readonly ILogger<ShiftController> _logger;
        private readonly IScheduleService _repo;

        public ShiftController(IScheduleService repo, ILogger<ShiftController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await _repo.Get());
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDetails(int? id)
        {
            if (id == null) return BadRequest();
            return Ok(await _repo.GetDetail(id ?? -1));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]ScheduleContainer schedule)
        {
            if (!ModelState.IsValid || schedule == null) return BadRequest("Invalid post data");
            try
            {
                var id = await _repo.Create(schedule);
                return Ok(id);
            }
            catch (BussinessException ex) //Fail bussiness check
            {
                Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                Utility.LogException(ex, _logger);
                return BadRequest("System error.");
            }
            catch (SqlException ex)
            {
                Utility.LogException(ex, _logger);
                return BadRequest("System error.");
            }

        }
    }
}
